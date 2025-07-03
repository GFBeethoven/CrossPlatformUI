using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class TempThumbnailCapture
{
    public static string Take()
    {
        string path = Path.Combine(Application.persistentDataPath, "TEMP_THUMBNAIL.png");

        if (File.Exists(path))
        {
            File.Delete(path);
        }

        ScreenCapture.CaptureScreenshot(path);

        return path;
    }
}
