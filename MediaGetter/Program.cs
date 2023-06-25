using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string url = Console.ReadLine(); ;
        string mediaLinksFilePath = "../../../Data/media_links.txt";
        string mediaFolderPath = "../../../Data/Media";

        // Download the web page content
        string htmlContent = DownloadWebPage(url);

        // Extract image and video URL links from the HTML content
        string[] mediaLinks = ExtractMediaLinks(htmlContent, "img", "video");

        // Save the links to a text file
        SaveMediaLinksToFile(mediaLinks, mediaLinksFilePath);

        Console.WriteLine("Media links saved to: " + mediaLinksFilePath);

        // Create the folder if it doesn't exist
        Directory.CreateDirectory(mediaFolderPath);

        // Download and save each media
        for (int i = 0; i < mediaLinks.Length; i++)
        {
            string mediaUrl = mediaLinks[i];
            string mediaFileName = $"{i + 1}.jpg";

            if (mediaUrl.EndsWith(".mp4"))
            {
                mediaFileName = $"{i + 1}.mp4";
            }

            string mediaPath = Path.Combine(mediaFolderPath, mediaFileName);

            DownloadMedia(mediaUrl, mediaPath);

            Console.WriteLine($"Downloaded media: {mediaFileName}");
        }

        Console.WriteLine("All media downloaded successfully.");
    }

    static string DownloadWebPage(string url)
    {
        using (WebClient client = new WebClient())
        {
            return client.DownloadString(url);
        }
    }

    static string[] ExtractMediaLinks(string htmlContent, params string[] mediaTags)
    {
        // Regex pattern to match media URLs
        string pattern = $"<(?:{string.Join("|", mediaTags)}).*?src=\"(?<url>http(s)?://.*?\\.(jpg|png|gif|jpeg|mp4))\".*?>";

        MatchCollection matches = Regex.Matches(htmlContent, pattern);
        string[] mediaLinks = new string[matches.Count];

        for (int i = 0; i < matches.Count; i++)
        {
            mediaLinks[i] = matches[i].Groups["url"].Value;
        }

        return mediaLinks;
    }

    static void SaveMediaLinksToFile(string[] mediaLinks, string filePath)
    {
        using (StreamWriter writer = new StreamWriter(filePath, false, Encoding.UTF8))
        {
            foreach (string link in mediaLinks)
            {
                writer.WriteLine(link);
            }
        }
    }

    static void DownloadMedia(string mediaUrl, string mediaPath)
    {
        using (WebClient client = new WebClient())
        {
            client.DownloadFile(mediaUrl, mediaPath);
        }
    }
}
