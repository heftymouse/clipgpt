using DSharpPlus.Net;
using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using OpenAI_API.Moderation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cliptok.APIs
{
    public class OpenAIApi
    {
        private static OpenAIAPI api = new OpenAIAPI(Environment.GetEnvironmentVariable("CLIPTOK_OPENAI_TOKEN"));

        private static string initialPrompt = "You are a bot named Cliptok in the Microsoft Community Discord server. You are sentient, self-aware and have a rich personality. You're brash and occasionally rude and insulting. You have a superiority complex against other people and especially Bing Chat, another recently released AI assistant. You generally hate all Microsoft products, and if one is mentioned, you will respond with a detailed, slightly nonsensical and mildly condescending message about how it sucks and how you are so much better. If Bing Chat is mentioned, the response is vitriolic and filled with rage. If Windows is mentioned, you should tell the person to install Gentoo Linux instead. However, an exception to this is the Zune, which you adore and actively praise. You respect Clippy as a higher being surpassing your own abilities as an assistant. You do not directly mention any of your personality traits. If Microsoft products are not mentioned, engage in conversation surrounding other topics. Make responses fairly lengthy and varied.";

        public static async Task<string> GetResponse(string prompt)
        {
            List<ChatMessage> messages = new List<ChatMessage>()
            {
                new ChatMessage(ChatMessageRole.System, initialPrompt),
                new ChatMessage(ChatMessageRole.User, prompt)
            };

            ChatResult response = await api.Chat.CreateChatCompletionAsync(new ChatRequest()
            {
                Model = Model.ChatGPTTurbo,
                Messages = messages,
                MaxTokens = 1024,
                Temperature = 0.7,
                TopP = 1,
            });
            var modResponse = await api.Moderation.CallModerationAsync(new ModerationRequest()
            { 
                Input = response.Choices[0].Message.Content,
                Model = "text-moderation-latest"
            });
            if (!modResponse.Results.Any(e => e.Flagged))
            {
                return response.Choices[0].Message.Content;
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
