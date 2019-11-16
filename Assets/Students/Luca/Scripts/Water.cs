using System;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Students.Luca.Scripts
{
    public class Water : MonoBehaviour
    {
        public float density = 1025;

        [ShowInInspector, SerializeField]
        private float heightIntensity = 1;
        public float HeightIntensity
        {
            get => heightIntensity;
            set
            {
                heightIntensity = value;
                HandleWaveIntensityChanged();
            }
        }
        
        [ShowInInspector, SerializeField]
        private float durationIntensity = 1;
        public float DurationIntensity
        {
            get => durationIntensity;
            set
            {
                durationIntensity = value;
                HandleWaveIntensityChanged();
            }
        }

        private Cloth waterCloth;

        private Tweener waveTween;

        private void Start()
        {
            waterCloth = GetComponent<Cloth>();
            //waveTween = transform.DOPunchPosition(new Vector3(0, HeightIntensity, 0), DurationIntensity, 10, 1f, false);
            //transform.DOMoveY(heightIntensity, time);
            //waveTween = DOTween.Punch(() => transform.position, (x) => transform.position = x, new Vector3(0,heightIntensity,0), durationIntensity, 10, 1f).SetLoops(-1,LoopType.Yoyo).Play();
            waveTween = DOTween.To(() => transform.position, (x) => transform.position = x, transform.position+(new Vector3(0,heightIntensity,0)), durationIntensity).SetLoops(-1,LoopType.Yoyo);
        }

        private void HandleWaveIntensityChanged()
        {
            Debug.Log("Values Changed "+waveTween);
            waveTween?.ChangeValues(new Vector3(0, HeightIntensity, 0), new Vector3(0, HeightIntensity, 0), DurationIntensity);
        }

        private void OnValidate()
        {
            HandleWaveIntensityChanged();
        }
    }
}