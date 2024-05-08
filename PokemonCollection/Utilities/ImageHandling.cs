using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;

public static class ImageHandling
{

    /// fixxx hhhhhhhhhhhhh
    public static async Task<string> ExtractTextFromImageUrlAsync(string url)
    {
        try
        {
            var imageBytes = await DownloadImageAsync(url);
            var text = await ExtractTextFromImageAsync(imageBytes);
            return text;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
            return string.Empty;
        }
    }

    public static async Task<string> ExtractTextFromImageAsync(byte[] imageBytes)
    {
        try
        {
            var result = await Task.Run(() =>
            {
                using (var engine = new TesseractEngine(@"C:\Program Files\Tesseract-OCR\tessdata", "eng", EngineMode.Default))
                {
                    using (var img = Pix.LoadFromMemory(imageBytes))
                    {
                        using (var page = engine.Process(img))
                        {
                            return page.GetText();
                        }
                    }
                }
            });

            return result;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
            return string.Empty;
        }
    }
    //////////////////////

    public static async Task<byte[]> DownloadImageAsync(string url)
    {
        try
        {
            using (HttpClient client = new HttpClient())
            {
                return await client.GetByteArrayAsync(url);
            }
        }
        catch (HttpRequestException ex)
        {
            MessageBox.Show("Error: " + ex.Message);
            return null;
        }
    }



    public static byte[] GetImageBytes(string imagePath)
    {
        try
        {
            // Check if the file exists
            if (!File.Exists(imagePath))
            {
                Console.WriteLine($"File not found: {imagePath}");
                return null;
            }

            // Read the image file into a byte array
            byte[] imageBytes;
            using (FileStream fs = new FileStream(imagePath, FileMode.Open, FileAccess.Read))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    imageBytes = br.ReadBytes((int)fs.Length);
                }
            }

            return imageBytes;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading image: {ex.Message}");
            return null;
        }
    }

    // Download Image, automatically a byte array :)
    public static void DownloadImage(string imageURL, Action<byte[]> callback)
    {
        try
        {
            using (WebClient webClient = new WebClient())
            {
                byte[] imageData = webClient.DownloadData(imageURL);
                callback?.Invoke(imageData);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error: " + ex.Message);
            callback?.Invoke(null);
        }
    }

    public static void HandleDownloadedImage(byte[] imageData, PictureBox pictureBox)
    {
        if (imageData != null)
        {
            using (MemoryStream ms = new MemoryStream(imageData))
            {
                pictureBox.Image = Image.FromStream(ms);
            }
        }
        else
        {
            pictureBox.Image = null;
        }
    }

    public static PictureBox GenerateBAPictureBox(string tag, byte[] image, int resX, int resY)
    {
        PictureBox pictureBox = new PictureBox()
        {
            Size = new Size(resY, resX),
            SizeMode = PictureBoxSizeMode.Zoom,
            BorderStyle = BorderStyle.FixedSingle,
            Margin = new Padding(5),
            Tag = tag
        };

        HandleDownloadedImage(image, pictureBox);

        return pictureBox;
    }


    // Simple one just for easy reuse
    public static Image ByteArrayToImage(byte[] byteArrayIn)
    {
        using (MemoryStream ms = new MemoryStream(byteArrayIn))
        {
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }
    }


}
