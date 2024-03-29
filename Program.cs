using System;
using System.Drawing;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Usage: Program.exe <inputImagePath> <desiredFileSize>");
            return;
        }

        string inputImagePath = args[0];
        long desiredFileSize = long.Parse(args[1]);

        string tempFileName = Path.ChangeExtension(inputImagePath, null) + "_temp.jpg";

        using (var image = Image.FromFile(inputImagePath))
        {
            long currentFileSize = new FileInfo(inputImagePath).Length;
            int quality = 95;

            while (currentFileSize > desiredFileSize)
            {
                int novaLargura = image.Width * quality / 100;
                int novaAltura = image.Height * quality / 100;


                using Image imagemReduzida = new Bitmap(novaLargura, novaAltura);
                using (Graphics g = Graphics.FromImage(imagemReduzida))
                {
                    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                    g.DrawImage(image, 0, 0, novaLargura, novaAltura);
                }


                using (MemoryStream msReduzida = new())
                {
                    imagemReduzida.Save(msReduzida, image.RawFormat);
                    File.WriteAllBytes(tempFileName, msReduzida.ToArray());
                }

                FileInfo outputFile = new(tempFileName);
                currentFileSize = outputFile.Length;

                if (currentFileSize > desiredFileSize)
                {
                    quality -= 5;
                }
            }
        }

        File.Delete(inputImagePath);
        File.Move(tempFileName, inputImagePath);
    }
}
