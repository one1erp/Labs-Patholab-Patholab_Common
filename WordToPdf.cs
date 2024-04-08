using System;
using System.IO;
using Microsoft.Office.Interop.Word;

namespace Patholab_Common
{
    public class WordToPdf
    {

        /// <summary>
        /// Convert word to pdf
        /// </summary>        
        /// <param name="fileName">Word file name</param>
        /// <param name="wordType">doc or docx</param>
        public static void Convert(string fileName, string wordType)
        {



            // Create a new Microsoft Word application object
            Application word = new Application();

            // C# doesn't have optional arguments so we'll need a dummy value
            object oMissing = System.Reflection.Missing.Value;

            try
            {
                //Get file
                FileInfo wordFile = new FileInfo(fileName);


                word.Visible = false;
                word.ScreenUpdating = false;


                if (wordFile != null)
                {
                    // Cast as Object for word Open method
                    Object filename = (Object)wordFile.FullName;

                    // Use the dummy value as a placeholder for optional arguments
                    Document doc = word.Documents.Open(ref filename, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing);
                    doc.Activate();

                    object outputFileName = wordFile.FullName.Replace("." + wordType, ".pdf");
                    object fileFormat = WdSaveFormat.wdFormatPDF;

                    // Save document into PDF Format
                    doc.SaveAs(ref outputFileName,
                        ref fileFormat, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing,
                        ref oMissing, ref oMissing, ref oMissing, ref oMissing);

                    // Close the Word document, but leave the Word application open.
                    // doc has to be cast to type _Document so that it will find the
                    // correct Close method.                
                    object saveChanges = WdSaveOptions.wdDoNotSaveChanges;
                    ((_Document)doc).Close(ref saveChanges, ref oMissing, ref oMissing);
                    doc = null;
                }


            }

            catch (Exception e)
            {
            }
            finally
            {
                // word has to be cast to type _Application so that it will find
                // the correct Quit method.
                ((_Application)word).Quit(ref oMissing, ref oMissing, ref oMissing);
                word = null;
            }
        }
    }
}


