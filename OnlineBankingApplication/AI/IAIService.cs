namespace OnlineBankingApplication.AI
{
    public interface IAIService
    {
        Task<string> GetResponseAsync(
            string userMessage,
            string userId);
    }
}