using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class VolumeController : MonoBehaviour
{
    public static VolumeController instance;
    private Volume volume;
    private Bloom bloom;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        instance = this;
        volume = GetComponent<Volume>();
        volume.profile.TryGet(out bloom);
    }
    public void changeBloomThreshold(float target)
    {
        bloom.threshold.Override(target);
    }
}
