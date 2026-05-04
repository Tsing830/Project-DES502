using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class FinalDoorCutscene : MonoBehaviour, IInteractable
{
    [Header("Video")]
    public VideoClip endingCutscene;
    public string endingCutsceneFileName = "ending cutscene.mp4";

    private Canvas canvas;
    private RawImage videoScreen;
    private VideoPlayer videoPlayer;
    private AudioSource audioSource;
    private RenderTexture renderTexture;
    private bool hasPlayed;

    public void Interact()
    {
        if (hasPlayed) return;

        hasPlayed = PlayCutscene();
    }

    private bool PlayCutscene()
    {
        CreateCutsceneUI();

        videoPlayer.Stop();
        videoPlayer.clip = endingCutscene;
        videoPlayer.url = string.Empty;

        if (endingCutscene == null)
        {
            string videoPath = GetVideoPath();
            if (!File.Exists(videoPath))
            {
                Debug.LogError("Ending cutscene not found: " + endingCutsceneFileName);
                return false;
            }

            videoPlayer.url = new System.Uri(videoPath).AbsoluteUri;
        }

        canvas.enabled = true;
        videoPlayer.Play();
        return true;
    }

    private void CreateCutsceneUI()
    {
        if (canvas != null) return;

        GameObject canvasObject = new GameObject("EndingCutsceneCanvas");
        canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = short.MaxValue;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = 0.5f;

        canvasObject.AddComponent<GraphicRaycaster>();

        renderTexture = new RenderTexture(1920, 1080, 0);

        GameObject screenObject = new GameObject("EndingCutsceneScreen");
        screenObject.transform.SetParent(canvasObject.transform, false);

        videoScreen = screenObject.AddComponent<RawImage>();
        videoScreen.texture = renderTexture;
        videoScreen.color = Color.white;

        RectTransform screenTransform = videoScreen.rectTransform;
        screenTransform.anchorMin = Vector2.zero;
        screenTransform.anchorMax = Vector2.one;
        screenTransform.offsetMin = Vector2.zero;
        screenTransform.offsetMax = Vector2.zero;

        audioSource = canvasObject.AddComponent<AudioSource>();

        videoPlayer = canvasObject.AddComponent<VideoPlayer>();
        videoPlayer.playOnAwake = false;
        videoPlayer.isLooping = false;
        videoPlayer.renderMode = VideoRenderMode.RenderTexture;
        videoPlayer.targetTexture = renderTexture;
        videoPlayer.audioOutputMode = VideoAudioOutputMode.AudioSource;
        videoPlayer.SetTargetAudioSource(0, audioSource);
        videoPlayer.loopPointReached += HideCutscene;

        canvas.enabled = false;
    }

    private string GetVideoPath()
    {
        string assetsPath = Path.Combine(Application.dataPath, endingCutsceneFileName);
        if (File.Exists(assetsPath)) return assetsPath;

        DirectoryInfo projectRoot = Directory.GetParent(Application.dataPath);
        if (projectRoot == null) return assetsPath;

        return Path.Combine(projectRoot.FullName, endingCutsceneFileName);
    }

    private void HideCutscene(VideoPlayer source)
    {
        if (canvas != null)
        {
            canvas.enabled = false;
        }
    }

    private void OnDestroy()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= HideCutscene;
        }

        if (renderTexture != null)
        {
            renderTexture.Release();
        }
    }
}
