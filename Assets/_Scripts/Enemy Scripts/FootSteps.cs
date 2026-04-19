using UnityEngine;

public class FootSteps : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] stoneClips;
    [SerializeField]
    private AudioClip[] mudClips;
    [SerializeField]
    private AudioClip[] grassClips;

    private AudioSource audioSource;
    private TerrainDetector terrainDetector;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        terrainDetector = new TerrainDetector();
    }

    public void PlayFootstepSound()
    {
        if (audioSource == null)
        {
            return;
        }

        AudioClip clip = GetRandomClip();
        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // Keep backward compatibility with any old animation event name.
    private void Step()
    {
        PlayFootstepSound();
    }

    private AudioClip GetRandomClip()
    {
        int terrainTextureIndex = terrainDetector.GetActiveTerrainTextureIdx(transform.position);

        switch(terrainTextureIndex)
        {
            case 0:
                if (stoneClips == null || stoneClips.Length == 0) return null;
                return stoneClips[UnityEngine.Random.Range(0, stoneClips.Length)];
            case 1:
                if (mudClips == null || mudClips.Length == 0) return null;
                return mudClips[UnityEngine.Random.Range(0, mudClips.Length)];
            case 2:
            default:
                if (grassClips == null || grassClips.Length == 0) return null;
                return grassClips[UnityEngine.Random.Range(0, grassClips.Length)];
        }
        
    }
}