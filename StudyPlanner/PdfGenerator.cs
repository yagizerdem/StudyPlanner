using DataContext.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Service;
using System.Data.Common;

namespace StudyPlanner
{
    public class PdfGenerator
    {
        private readonly NotesController notesController;
        private readonly TimableEventController timableEventController;
        private readonly StudyResourcesController studyResourcesController;

        public PdfGenerator()
        {
            this.notesController = Program.ServiceProvider.GetRequiredService<NotesController>();
            this.timableEventController = Program.ServiceProvider.GetRequiredService<TimableEventController>();
            this.studyResourcesController = Program.ServiceProvider.GetRequiredService<StudyResourcesController>();

            QuestPDF.Settings.License = LicenseType.Community;

        }

        public async Task GeneratePdf()
        {

            List<NotesModel> notes = (await notesController.GetNotes()).Data;
            List<TimableEventModel> timableEvents = (await timableEventController.Get()).Data;
            List<SubjectModel> subjects = (await studyResourcesController.GetSubjects()).Data;
            try
            {

                byte[] pdf = Document.Create(container =>
                 {
                     container.Page(page =>
                     {
                         page.Size(PageSizes.A4);
                         page.Margin(2, Unit.Centimetre);
                         page.PageColor(Colors.White);
                         page.DefaultTextStyle(x => x.FontSize(20));


                         // 📌 Stylish Header
                         page.Header().BorderBottom(2)
                             .Padding(10)
                             .Background(Colors.Blue.Lighten3)
                             .AlignCenter()
                             .Text("📌 Study Plan - Notes")
                             .SemiBold().FontSize(28).FontColor(Colors.White);


                         page.Content().Column(column =>
                         {

                             // 📄 Notes Section
                             column.Item()
                                 .BorderBottom(2)
                                 .PaddingBottom(10)
                                 .Text("📝 Notes")
                                 .SemiBold()
                                 .FontSize(24)
                                 .FontColor(Colors.Blue.Darken1);

                             for (int i = 0; i < notes.Count; i++)
                             {
                                 NotesModel note = notes[i];

                                 column.Item()
                                     .Padding(5)
                                     .Border(2)
                                     .BorderColor(Colors.Blue.Lighten2)
                                     .Background(i % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White)
                                     .Padding(15)
                                     .Column(card =>
                                     {
                                         // Title
                                         card.Item().Text(note.Title)
                                             .SemiBold()
                                             .FontSize(20)
                                             .FontColor(Colors.Blue.Darken2);

                                         // Divider Line
                                         card.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Blue.Lighten2);

                                         // Body
                                         card.Item().Text(note.Body)
                                             .FontSize(16)
                                             .FontColor(Colors.Black);
                                     });
                             }



                             // 📅 Events Section
                             column.Item()
                                 .BorderBottom(2)
                                 .PaddingBottom(10)
                                 .PaddingTop(20)
                                 .Text("📅 Events")
                                 .SemiBold()
                                 .FontSize(24)
                                 .FontColor(Colors.Blue.Darken1);

                             for (int i = 0; i < timableEvents.Count; i++)
                             {
                                 TimableEventModel timableEvent = timableEvents[i];

                                 column.Item()
                                     .Padding(5)
                                     .Border(2)
                                     .BorderColor(Colors.Blue.Lighten2)
                                     .Background(i % 2 == 0 ? Colors.Grey.Lighten3 : Colors.White)
                                     .Padding(15)
                                     .Column(card =>
                                     {
                                         // Title
                                         card.Item().Text(timableEvent.Name)
                                             .SemiBold()
                                             .FontSize(20)
                                             .FontColor(Colors.Blue.Darken2);

                                         // Divider Line
                                         card.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Blue.Lighten2);

                                         // Event Time
                                         card.Item().Text($"📅 {timableEvent.EventTime}")
                                             .FontSize(14)
                                             .FontColor(Colors.Black);
                                     });
                             }


                             column.Item()
                             .BorderBottom(2)
                             .PaddingBottom(10)
                             .PaddingTop(20)
                             .Text("📚 Study Plan - Subjects Overview")
                             .SemiBold()
                             .FontSize(24)
                             .FontColor(Colors.Blue.Darken1);




                             foreach (var subject in subjects)
                             {
                                 column.Item()
                                     .Padding(10)
                                     .Border(2)
                                     .BorderColor(Colors.Blue.Lighten2)
                                     .Background(Colors.Grey.Lighten3) // Light background for subject
                                     .Column(subColumn =>
                                     {
                                         // 📖 Subject Name
                                         subColumn.Item().Text($"📖 {subject.Name}")
                                             .SemiBold()
                                             .FontSize(22)
                                             .FontColor(Colors.Blue.Darken2);

                                         // Divider Line
                                         subColumn.Item().PaddingVertical(5).LineHorizontal(1).LineColor(Colors.Blue.Lighten2);

                                         // Loop through Units
                                         foreach (var unit in subject.Unit)
                                         {
                                             subColumn.Item()
                                                 .PaddingLeft(20) // Indent for Units
                                     .BorderLeft(1).
                                                             BorderColor(Colors.Green.Lighten2)
                                                 .Padding(5)
                                                 .Column(unitColumn =>
                                                 {
                                                     // 📂 Unit Name
                                                     unitColumn.Item().Text($"📂 {unit.Name}")
                                                         .SemiBold()
                                                         .FontSize(18)
                                                         .FontColor(Colors.Green.Darken2);

                                                     // Loop through Resources
                                                     foreach (var resource in unit.Resources)
                                                     {
                                                         unitColumn.Item()
                                                             .PaddingLeft(40) // Indent for Resources
                                                             .BorderLeft(1).
                                                             BorderColor(Colors.Green.Lighten2)
                                                             .Padding(3)
                                                             .Row(row =>
                                                             {
                                                                 // 📜 Resource Name
                                                                 row.ConstantItem(10).Text("•"); // Bullet point
                                                                 row.RelativeItem().Text(resource.Name)
                                                                     .FontSize(16)
                                                                     .FontColor(Colors.Black);
                                                             });
                                                     }
                                                 });
                                         }
                                     });
                             }





                         });



                         page.Footer()
                   .AlignCenter()
                   .Text("Generated using QuestPDF 🚀")
                   .FontSize(12)
                   .Italic();


                     });
                 }).GeneratePdf();


                string basePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                string filePath = Path.Combine(basePath, "StudyManager.pdf");

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }

                File.WriteAllBytes(filePath, pdf);

                ToastNotification.ShowSuccessToast("pdf generated");

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);

                ToastNotification.ShowErrorToast("error occured");
            }
        }

    }
}
