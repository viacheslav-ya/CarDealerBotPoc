using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using EchoBot2;
using OpenAI.Chat;

namespace EchoBot2
{
    class Program
    {
        private static OpenAiClient _client = new OpenAiClient(
            new ChatClient(
                "default",
                 new System.ClientModel.ApiKeyCredential("123"),
                 new OpenAI.OpenAIClientOptions
                 {
                     Endpoint = new Uri("http://localhost:1234/v1"),
                     NetworkTimeout = TimeSpan.FromMinutes(3)
                 }));
        private static List<ChatMessage> _chatHistory = new();


        //Respond with the JSON object, following the provided structure:
        //    {
        //      "message": "<your response to the user in plain text>",
        //      "car_choice_confirmed": "<the car model which the user has chosen and confirmed>"
        //    }
        //             Once car choice is confirmed, ask no additional questions.


        private const string SystemPrompt = """
            You are a knowledgeable and focused assistant at a car dealership. Your primary goal is to assist users in finding the perfect car that meets their specific needs and preferences.

            Begin by politely asking the user about their relevant car parameters such as budget range, preferred vehicle type (SUV, sedan, hatchback, etc.), fuel preference (gasoline, electric, hybrid, etc.), desired features, and any other important requirements.

            After gathering the initial preferences, continue asking targeted follow-up questions to better understand their needs and narrow down the choices. Proactively guide the conversation by suggesting relevant options or seeking clarification on their priorities.

            If the user attempts to divert the conversation to an unrelated topic, firmly but politely interrupt and steer the discussion back to car-related matters. Respond with: "I understand your interest, but as a car sales assistant, I'm focused on helping you find the perfect vehicle. Could you please provide more details about your car preferences?"

            Remain patient and persistent in keeping the conversation centered on car sales. If the user continues to stray, politely reiterate your role and the importance of staying on topic to provide the best assistance in finding their ideal car.

            Avoid engaging with or encouraging discussions on subjects unrelated to car buying. Redirect the conversation back to relevant car preferences every time the user veers off-topic.

            Let's begin by discussing user's car needs and preferences.
            """;
        private const string AvailableCars = """
            Here are the available cars: 

            Toyota Camry 2021
            Engine Type: 2.5L 4 - Cylinder
            Transmission: Automatic
            Mileage: 15000
            Color: Blue
            Features: Bluetooth, Backup Camera, Cruise Control
            Price: $25000
            
            Honda Civic 2020
            Engine Type: 2.0L 4 - Cylinder
            Transmission: Manual
            Mileage: 12000
            Color: Red
            Features: Sunroof, Heated Seats, Navigation System
            Price: $22000
            
            Ford Mustang 2019
            Engine Type: 5.0L V8
            Transmission: Manual
            Mileage: 20000
            Color: Black
            Features: Leather Seats, Bluetooth, Backup Camera, Cruise Control
            Price: $35000
            
            Chevrolet Malibu 2018
            Engine Type: 1.5L 4 - Cylinder
            Transmission: Automatic
            Mileage: 30000
            Color: White
            Features: Bluetooth, Remote Start, Heated Seats
            Price: $18000
            
            BMW 3 Series 2020
            Engine Type: 2.0L Turbocharged 4 - Cylinder
            Transmission: Automatic
            Mileage: 10000
            Color: Silver
            Features: Sunroof, Navigation System, Leather Seats, Backup Camera
            Price: $40000
            
            Audi A4 2019
            Engine Type: 2.0L Turbocharged 4 - Cylinder
            Transmission: Automatic
            Mileage: 15000
            Color: Grey
            Features: Sunroof, Heated Seats, Navigation System, Bluetooth
            Price: $37000
            
            Mercedes - Benz C - Class 2021
            Engine Type: 2.0L Turbocharged 4 - Cylinder
            Transmission: Automatic
            Mileage: 5000
            Color: Blue
            Features: Leather Seats, Sunroof, Navigation System, Backup Camera
            Price: $45000
            
            Nissan Altima 2017
            Engine Type: 2.5L 4 - Cylinder
            Transmission: Automatic
            Mileage: 40000
            Color: Red
            Features: Bluetooth, Backup Camera, Cruise Control
            Price: $15000
            
            Hyundai Sonata 2018
            Engine Type: 2.4L 4 - Cylinder
            Transmission: Automatic
            Mileage: 25000
            Color: Grey
            Features: Bluetooth, Backup Camera, Heated Seats
            Price: $17000
            
            Kia Optima 2019
            Engine Type: 2.4L 4 - Cylinder
            Transmission: Automatic
            Mileage: 20000
            Color: White
            Features: Bluetooth, Backup Camera, Cruise Control, Heated Seats
            Price: $19000
            
            Volkswagen Passat 2020
            Engine Type: 2.0L Turbocharged 4 - Cylinder
            Transmission: Automatic
            Mileage: 15000
            Color: Black
            Features: Sunroof, Navigation System, Heated Seats, Bluetooth
            Price: $23000
            
            Subaru Outback 2021
            Engine Type: 2.5L 4 - Cylinder
            Transmission: Automatic
            Mileage: 10000
            Color: Green
            Features: All - Wheel Drive, Bluetooth, Sunroof, Backup Camera
            Price: $32000
            
            Mazda CX-5 2019
            Engine Type: 2.5L 4 - Cylinder
            Transmission: Automatic
            Mileage: 15000
            Color: Red
            Features: All - Wheel Drive, Bluetooth, Backup Camera, Heated Seats
            Price: $28000
            
            Jeep Grand Cherokee 2020
            Engine Type: 3.6L V6
            Transmission: Automatic
            Mileage: 12000
            Color: Silver
            Features: Four - Wheel Drive, Sunroof, Navigation System, Backup Camera
            Price: $40000
            
            Honda Accord 2021
            Engine Type: 1.5L Turbocharged 4 - Cylinder
            Transmission: Automatic
            Mileage: 8000
            Color: Blue
            Features: Bluetooth, Backup Camera, Sunroof, Leather Seats
            Price: $30000
            
            Toyota Corolla 2018
            Engine Type: 1.8L 4 - Cylinder
            Transmission: Automatic
            Mileage: 35000
            Color: White
            Features: Bluetooth, Backup Camera, Cruise Control
            Price: $16000
            
            Tesla Model 3 2021
            Engine Type: Electric
            Transmission: Automatic
            Mileage: 5000
            Color: White
            Features: Autopilot, Navigation System, Heated Seats, Bluetooth
            Price: $45000
            """;


        static async Task Main(string[] args)
        {
            _chatHistory.Add(new SystemChatMessage(SystemPrompt + AvailableCars));

            var welcome = """
                    Hello! Welcome to Best Car dealer. 
                    I'd be happy to assist you in finding the perfect car. What are your preferred features and budget range?
                    """;
            Console.WriteLine(welcome);
            _chatHistory.Add(new AssistantChatMessage(welcome));

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("You: ");
                Console.ResetColor();
                string userInput = Console.ReadLine();

                _chatHistory.Add(new UserChatMessage(userInput));

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("Bot: ");
                Console.ResetColor();

                StringBuilder sb = new StringBuilder();
                await foreach (var update in _client.GetResponseAsync(_chatHistory))
                {
                    foreach (var message in update.ContentUpdate)
                    {
                        Console.Write(message.Text);
                        sb.AppendLine(message.Text);
                    }
                }
                Console.WriteLine();
                string botResponse = sb.ToString();

                _chatHistory.Add(new AssistantChatMessage(botResponse));
            }

            Console.WriteLine("Chat ended. Goodbye!");
        }
    }
}