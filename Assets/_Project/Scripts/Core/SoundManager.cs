using UnityEngine;

public class SoundManager : BaseManager<SoundManager>
{
    [Header("Audio Sources")]
    public AudioSource bgmSource;
    public AudioSource sfxSource;

    [Header("Music Clips (BGM)")]
    public AudioClip homeBGM;
    public AudioClip gameplayBGM;

    [Header("SFX Clips")]
    public AudioClip rifleShotSFX;
    public AudioClip shotgunShotSFX;
    public AudioClip bombExplosionSFX;
    public AudioClip zombieAttackSFX;
    public AudioClip zombieDeadSFX;
    public AudioClip buttonClickSFX;
    [Header("Bomb UI/Ticking")]
    public AudioClip bombTickSFX; 

    protected override void Awake()
    {
        base.Awake();
        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.loop = true;
            bgmSource.playOnAwake = false;
        }

        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
        }
    }

    public void PlayBGM(AudioClip clip)
    {
        if (clip == null || bgmSource == null) return;
        if (bgmSource.clip == clip) return; 
        
        bgmSource.clip = clip;
        bgmSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null || sfxSource == null)  return;
        sfxSource.PlayOneShot(clip);
    }

    public void PlayRifleShot() => PlaySFX(rifleShotSFX);
    public void PlayShotgunShot() => PlaySFX(shotgunShotSFX);
    public void PlayBombExplosion() => PlaySFX(bombExplosionSFX);
    public void PlayZombieAttack() => PlaySFX(zombieAttackSFX);
    public void PlayZombieDead() => PlaySFX(zombieDeadSFX);
    public void PlayButtonClick() => PlaySFX(buttonClickSFX);
}
