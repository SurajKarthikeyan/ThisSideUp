using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomizedSFX : MonoBehaviour
{
    [Tooltip("Picks a random sound from this list to play")]
    [SerializeField] List<AudioClip> sounds;
    [Tooltip("The range in which the random pitch of the sound will fall")]
    [SerializeField] Vector2 pitchRange;

    AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        source = GetComponent<AudioSource>();
        source.clip = sounds[Random.Range(0, sounds.Count)];
        source.pitch = Random.Range(pitchRange.x, pitchRange.y);
        source.Play();
        StartCoroutine(nameof(DestroyAfterSeconds), source.clip.length + 0.1f);
    }

    private IEnumerator DestroyAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
        yield break;
    }
}
