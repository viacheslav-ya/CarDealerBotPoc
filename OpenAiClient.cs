using OpenAI.Chat;
using System;
using System.ClientModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EchoBot2
{
    public class OpenAiClient
    {
        private readonly ChatClient _chatClient;

        public OpenAiClient(ChatClient chatClient)
        {
            _chatClient = chatClient;
        }

        public AsyncResultCollection<StreamingChatCompletionUpdate> GetResponseAsync(IReadOnlyList<ChatMessage> messages)
        {
            AsyncResultCollection<StreamingChatCompletionUpdate> completeResponse = _chatClient.CompleteChatStreamingAsync(
                messages,
                new ChatCompletionOptions
                {
                    Temperature = 0.2f,
                    MaxTokens = -1,
                    //FrequencyPenalty = 0,
                    //PresencePenalty = 0.6f
                });

            return completeResponse;
            //var result = completeResponse.Value.Content[0].Text;

            //if (result.StartsWith("```json"))
            //{
            //    result = result.Substring(7);
            //}
            //if (result.EndsWith("```"))
            //{
            //    result = result.Substring(0, result.Length - 3);
            //}

            //return result;
        }
    }
}
