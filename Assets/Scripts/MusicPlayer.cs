using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    private static MusicPlayer instance;

    void Awake()
    {
        // Cegah duplikat musik saat pindah scene
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // tetap hidup di scene berikut
        }
        else
        {
            Destroy(gameObject); // hapus duplikat
        }
    }
}
