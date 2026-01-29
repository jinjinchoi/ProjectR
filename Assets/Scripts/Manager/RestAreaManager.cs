using UnityEngine;

public class RestAreaManager : MonoBehaviour
{
    public static RestAreaManager Instance { get; private set; }

    private int day;
    

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        
    }
    private void ProcessDay()
    {
        
    }
}
