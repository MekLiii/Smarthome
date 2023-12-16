// using Smarthome.Bulbs.interfaces;
//
// namespace Smarthome.SpeechSystem;
//
// public class SpeachCases
// {
//     private readonly IBulbsService _bulbsService;
//
//     public SpeachCases(IBulbsService bulbsService)
//     {
//         _bulbsService = bulbsService ?? throw new ArgumentNullException(nameof(bulbsService));
//     }
//
//     public string[] SpeechCasesArray = new string[]
//     {
//         "on", "turn off", "brightness to fifty", "brightness to hundred", "brightness","Włącz"
//     };
//
//     public void Cases(string expression,float confidence)
//     {
//         if(confidence < 0.94)
//             return;
//         try
//         {
//             if (_bulbsService == null)
//             {
//                 Console.WriteLine("Error: IBulbsService is null.");
//                 return;
//             }
//
//             var dimBulbDto = new IBulbRequest()
//             {
//                 ltype = "white",
//                 colorTypes = new ColorTypes()
//                 {
//                     Br = 1,
//                 }
//             };
//             
//             expression = expression.ToLower();
//             switch (expression)
//             {
//                 case "on":
//                     _bulbsService.SwitchBulb("1001e77fdb", "on");
//                     break;
//                 case "turn off":
//                     _bulbsService.SwitchBulb("1001e77fdb", "off");
//                     break;
//                 case "brightness":
//                     _bulbsService.DimBulb("1001e77fdb", dimBulbDto);
//                     break;
//                 case "brightness to fifty":
//                   
//                     _bulbsService.DimBulb("1001e77fdb",
//                         new IBulbRequest()
//                         {
//                             ltype = "white",
//                             colorTypes = new ColorTypes()
//                             {
//                                 Br = 50,
//                                 Ct = 0,
//                             }
//                         });
//                     break;
//                 case "brightness to hundred":
//                     _bulbsService.DimBulb("1001e77fdb", new IBulbRequest()
//                     {
//                         ltype = "white",
//                         colorTypes = new ColorTypes()
//                         {
//                             Br = 100,
//                         }
//                     });
//                     break;
//                 default:
//                     Console.WriteLine("Error: Expression not found.");
//                     break;
//             }
//         }
//         catch (Exception e)
//         {
//             Console.WriteLine(e);
//             throw;
//         }
//     }
// }