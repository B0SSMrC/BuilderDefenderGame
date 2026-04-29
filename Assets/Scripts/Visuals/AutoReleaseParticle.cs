using UnityEngine;
using UnityEngine.AddressableAssets;

[RequireComponent(typeof(ParticleSystem))]
public class AutoReleaseParticle : MonoBehaviour
{
    private ParticleSystem ps;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        
        // 确保原生设定没有抢先 Destroy
        var main = ps.main;
        main.stopAction = ParticleSystemStopAction.None; 
    }

    private void Update()
    {
        // 检查粒子是否播放完毕
        if (ps != null && !ps.IsAlive(true))
        {
            //  播放完毕后，交还给 Addressables 释放
            Addressables.ReleaseInstance(gameObject);
        }
    }
}