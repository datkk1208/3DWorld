using UnityEngine;

[RequireComponent(typeof(AudioSource), typeof(Animator))]
public class PlayerAudioManager : MonoBehaviour
{
    [Header("Combat Sounds")]
    public AudioClip[] AttackSounds;
    public AudioClip HitSound;
    public AudioClip DeathSound;

    [Header("Movement Sounds (Dạng dài liên tục)")]
    public AudioClip WalkSoundLoop;
    public AudioClip RunSoundLoop;

    private AudioSource _combatAudioSource;
    private AudioSource _movementAudioSource;
    private Animator _animator;
    private PlayerController _player;

    // Biến lưu vị trí để đo quãng đường thật
    private Vector3 _lastPosition;

    private void Awake()
    {
        _combatAudioSource = GetComponent<AudioSource>();
        _animator = GetComponent<Animator>();
        _player = GetComponent<PlayerController>();

        _movementAudioSource = gameObject.AddComponent<AudioSource>();
        _movementAudioSource.loop = true;
        _movementAudioSource.playOnAwake = false;
        _movementAudioSource.volume = 0.5f;

        _lastPosition = transform.position;
    }

    private void Update()
    {
        if (_player == null) return;
        HandleMovementSound();
    }

    private void HandleMovementSound()
    {
        // 1. Đo tốc độ DI CHUYỂN THỰC TẾ (khoảng cách giữa frame trước và frame này)
        Vector3 currentPosFlat = new Vector3(transform.position.x, 0, transform.position.z);
        Vector3 lastPosFlat = new Vector3(_lastPosition.x, 0, _lastPosition.z);

        float actualSpeed = Vector3.Distance(currentPosFlat, lastPosFlat) / Time.deltaTime;

        // Lưu lại vị trí cho frame tiếp theo
        _lastPosition = transform.position;

        // 2. Chỉ phát tiếng khi: Chạm đất + Tốc độ xê dịch > 0.1m/s + Không bị choáng
        // (Nếu bạn đang chém mà nhân vật đứng im -> actualSpeed = 0 -> Sẽ tự tắt tiếng, không cần xét Combo)
        bool isMoving = _player.IsGrounded() && actualSpeed > 0.1f && !_player.IsStunned;

        if (isMoving)
        {
            // 3. Phân loại chuẩn Walk/Run theo tốc độ vật lý
            AudioClip currentClip = actualSpeed > (_player.WalkSpeed + 1f) ? RunSoundLoop : WalkSoundLoop;

            // Đổi file nếu chuyển từ đi sang chạy (hoặc ngược lại)
            if (_movementAudioSource.clip != currentClip)
            {
                _movementAudioSource.clip = currentClip;
                _movementAudioSource.Play();
            }
            // Bật nếu chưa bật
            else if (!_movementAudioSource.isPlaying)
            {
                _movementAudioSource.Play();
            }
        }
        else
        {
            // Đứng im hoặc nhảy lên -> Tạm dừng phát
            if (_movementAudioSource.isPlaying)
            {
                _movementAudioSource.Pause();
            }
        }
    }

    // --- CÁC HÀM CHIẾN ĐẤU ---
    public void PlayAttackSound()
    {
        int currentCombo = _animator.GetInteger("ComboCounter");
        if (currentCombo >= 1 && currentCombo <= AttackSounds.Length)
        {
            AudioClip clip = AttackSounds[currentCombo - 1];
            if (clip != null) _combatAudioSource.PlayOneShot(clip);
        }
    }

    public void PlayHitSound() { if (HitSound != null) _combatAudioSource.PlayOneShot(HitSound); }
    public void PlayDeathSound() { if (DeathSound != null) _combatAudioSource.PlayOneShot(DeathSound); }

    // Rỗng để chống lỗi Animation Event cũ
    public void PlayFootstep() { }
    public void PlayFootstepEvent() { }
}