using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.SemanticFunctions;
using DMSRAG.Web.Data;
using Microsoft.SemanticKernel.Orchestration;
using Microsoft.SemanticKernel.Memory;
using HtmlAgilityPack;
using System.Threading;

using DMSRAG.Web.Helpers;
using Microsoft.SemanticKernel.SkillDefinition;
using OpenAI.Interfaces;

namespace DMSRAG.Web.Services
{
    public class QAUrlService
    {
        public string SkillName { get; set; } = "QAUrlSkill";
        public string FunctionName { set; get; } = "QAUrl";
        int MaxTokens { set; get; }
        double Temperature { set; get; }
        double TopP { set; get; }
        public bool IsProcessing { get; set; } = false;
        const string COLLECTION = "PageCollection";
        Dictionary<string, ISKFunction> ListFunctions = new Dictionary<string, ISKFunction>();
        HashSet<string> Ids = new HashSet<string>();
        IKernel kernel { set; get; }
        public int ContentCount { get; internal set; } = 0;
        TokenizerService tokenSvc;
        public QAUrlService(IOpenAIService service)
        {
            try
            {
                // Configure AI backend used by the kernel
                var (model, apiKey, orgId) = AppConstants.GetSettings();
                kernel = new KernelBuilder()
    .WithOpenAITextEmbeddingGenerationService(modelId: "text-embedding-ada-002", apiKey: apiKey, orgId: orgId, serviceId: "embedding")
    .WithOpenAITextCompletionService(modelId: model, apiKey: apiKey, orgId: orgId, serviceId: "davinci")
    .Build();
                kernel.UseMemory(new VolatileMemoryStore());
                SetupSkill();
                tokenSvc = new TokenizerService(service);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        public void SetupSkill(int MaxTokens = 2000, double Temperature = 0.2, double TopP = 0.5)
        {
            this.MaxTokens = MaxTokens;
            this.Temperature = Temperature;
            this.TopP = TopP;

            string skPrompt = """
Use the following pieces of context to answer the question at the end. If you don't know the answer, don't try to make up an answer and answer with 'I don't know'. Answer in the language that used for the question.

{{$konteks}}

Question: {{$input}}
Answer:
""";

            var promptConfig = new PromptTemplateConfig
            {
                Completion =
    {
        MaxTokens = this.MaxTokens,
        Temperature = this.Temperature,
        TopP = this.TopP,
    }
            };

            var promptTemplate = new PromptTemplate(
    skPrompt,                        // Prompt template defined in natural language
    promptConfig,                    // Prompt configuration
    kernel                           // SK instance
);


            var functionConfig = new SemanticFunctionConfig(promptConfig, promptTemplate);

            var QAUrlFunction = kernel.RegisterSemanticFunction(SkillName, FunctionName, functionConfig);
            ListFunctions.Add(FunctionName, QAUrlFunction);
        }
        public async Task<string> Answer(string question)
        {
            if (ContentCount <= 0) throw new Exception("Please add content at least 1 page url");
            string Result = string.Empty;
            if (IsProcessing) return Result;

            try
            {
                TokenHelper.CheckMaxToken(this.MaxTokens, question);
                IsProcessing = true;

                var results = kernel.Memory.SearchAsync(COLLECTION, question, limit: 2).ToBlockingEnumerable();

                var context = new ContextVariables();
                var ctx = string.Empty;
                if (results.Any())
                {
                    var relevantData = results.Select(r => r.Metadata.Text);
                    foreach(var item in relevantData)
                    {
                        var token = await tokenSvc.GetTokens(ctx+item);
                        if (token.Count > AppConstants.TokenLimit/2)
                        {
                            break;
                        }
                        ctx += item + "\n";
                    }
                }
                else
                {
                    ctx = "No context found for this question.";
                }
                       
                context.Set("konteks", ctx);
                context.Set("input", question);
                //var fun = ListFunctions[FunctionName];
                //var result = await fun.InvokeAsync(new SKContext(context));
                var result = await kernel.RunAsync(context, ListFunctions[FunctionName]);
                if (result.ErrorOccurred) throw new Exception(result.LastErrorDescription);
                Result = result.Result;

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return ex.ToString();
            }
            finally
            {
                IsProcessing = false;
            }
            return Result;

        }



        public async Task AddPageUrl(HttpClient client, string url, string contentSelector)
        {
            var content = await client.GetStringAsync(url);
            var title = string.Empty;

            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            var mainElement = doc.DocumentNode.SelectSingleNode(contentSelector);
            title = mainElement.SelectSingleNode("//h1").InnerText;
            content = mainElement.InnerText;

            await kernel.Memory.SaveInformationAsync(COLLECTION, content, url, title);
            Ids.Add(url);
            ContentCount++;
        }

        public async Task AddContent(string title, string content)
        {
            ContentCount++;
            var url = $"doc-{title}-page-{ContentCount}";

            await kernel.Memory.SaveInformationAsync(COLLECTION, content, url, title);

        }

        public async Task Reset()
        {
            if (ContentCount <= 0) return;
            foreach (var url in Ids)
            {
                await kernel.Memory.RemoveAsync(COLLECTION, url);
            }
            ContentCount = 0;
        }
    }
}
