using UnityEngine;
using UnityEngine.AddressableAssets;

public class AutoReleaseParticle : MonoBehaviour
{
    public float lifeTime = 2f; 
    private float timer;

    private void OnEnable()
    {
        // 每次生成或重新激活时，重置倒计时
        timer = lifeTime; 
    }

    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            Addressables.ReleaseInstance(gameObject);
        }
    }
}