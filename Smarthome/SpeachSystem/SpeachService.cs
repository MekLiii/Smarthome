// using System;
// using System.Globalization;
// using System.Speech.Recognition;
// using Smarthome.Bulbs.interfaces;
//
// namespace Smarthome.SpeechSystem
// {
//     public class SpeechService
//     {
//         private readonly IBulbsService _bulbsService;
//
//         public SpeechService(IBulbsService bulbsService)
//         {
//             Console.WriteLine("### SPEACH SERVICE STARTED");
//             _bulbsService = bulbsService ?? throw new ArgumentNullException(nameof(bulbsService));
//         }
//         
//
//
//         public async Task TranscribeAudioAsync()
//         {
//             var speechCases = new SpeachCases(_bulbsService);
//             if (speechCases == null || speechCases.SpeechCasesArray == null)
//             {
//                 Console.WriteLine("Error: SpeechCases or SpeechCasesArray is null.");
//                 return;
//             }
//             var recognizer = new SpeechRecognitionEngine(new CultureInfo("pl-PL"));
//             Console.WriteLine("### STARTED LISTENING");
//             using var exit = new ManualResetEvent(false);
//             var choices = new Choices();
//             choices.Add(speechCases.SpeechCasesArray);
//            
//             var gb = new GrammarBuilder();
//             gb.Append(choices);
//
//             var g = new Grammar(gb);
//             recognizer.LoadGrammar(g);
//             recognizer.SpeechRecognized += (s, e) =>
//             {
//                 speechCases.Cases(e.Result.Text, e.Result.Confidence);
//
//                 Console.WriteLine($"Recognized: {e.Result.Text}, Confidence: {e.Result.Confidence}");
//             };
//             recognizer.SpeechRecognitionRejected += (s, e) =>
//             {
//                 Console.WriteLine($"Speech rejected: {e.Result.Text}");
//             };
//
//             try
//             {
//                 recognizer.SetInputToDefaultAudioDevice();
//                 recognizer.RecognizeAsync(RecognizeMode.Multiple);
//           
//                 
//                 await Task.Run(() => exit.WaitOne());
//             }
//             catch (Exception ex)
//             {
//                 Console.WriteLine(ex.Message);
//             }
//             finally
//             {
//                 recognizer.Dispose();
//             }
//         }
//     }
// }