using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public enum ObstacleType { Mine };
    public ObstacleType obstacleType;

    //[SerializeField] private Sprite secondSprite;

    AudioSource audioSource;
    SpriteRenderer spriteRenderer;
    CircleCollider2D circleCollider;
    [SerializeField] public ParticleSystem particleSystem;
    public float explosionForce;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
    }

    public void Activate()
    {
        switch (obstacleType)
        {
            case ObstacleType.Mine:
                spriteRenderer.enabled = false;
                circleCollider.enabled = false;
                audioSource.Play();
                particleSystem.Play();
                break;
        }
    }
}
