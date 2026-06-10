using UnityEngine;
using UnityEngine.UI;
using System.IO;
using TMPro;

public class CameraCapture : MonoBehaviour
{
    [SerializeField] private RawImage _settingsDisplayImage;
    [SerializeField] private RawImage _playerDisplayImage;
    [SerializeField] private Button _captureButton;

    private const string ImagePathKey = "CapturedImagePath";

    private void Start()
    {
        _captureButton.onClick.AddListener(OpenCamera);
        LoadSavedImage();
    }

    private void OpenCamera()
    {
        if (NativeCamera.IsCameraBusy())
            return;

        NativeCamera.Permission permission = NativeCamera.TakePicture((path) =>
        {
            if (string.IsNullOrEmpty(path))
                return;

            Debug.Log("Image saved at: " + path);
            SaveImagePath(path);
            LoadImage(path);
        });

        Debug.Log("Camera permission: " + permission);
    }

    private void SaveImagePath(string path)
    {
        PlayerPrefs.SetString(ImagePathKey, path);
        PlayerPrefs.Save();
    }

    private void LoadSavedImage()
    {
        if (PlayerPrefs.HasKey(ImagePathKey))
        {
            string savedPath = PlayerPrefs.GetString(ImagePathKey);
            LoadImage(savedPath);
        }
    }

    private void LoadImage(string path)
    {
        if (File.Exists(path))
        {
            byte[] imageBytes = File.ReadAllBytes(path);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(imageBytes);
            _settingsDisplayImage.texture = texture;
            _playerDisplayImage.texture = texture;
        }
        else
        {
            Debug.LogError("Image file not found at path: " + path);
        }
    }
}
