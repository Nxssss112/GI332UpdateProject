using UnityEngine;

public class Particle : MonoBehaviour
{
    // ตัวแปรสำหรับเก็บอ้างอิง Particle System
    private ParticleSystem _particleSystem;

    void Awake()
    {
        // ดึง Component ParticleSystem มาเก็บไว้ในตัวแปร
        _particleSystem = GetComponent<ParticleSystem>();
    }

    void Start()
    {
        // สั่งให้ Particle เริ่มทำงานทันทีที่เข้าโหมด Play
        if (_particleSystem != null)
        {
            _particleSystem.Play();
        }
        else
        {
            Debug.LogWarning("ไม่พบ Particle System ใน Object นี้: " + gameObject.name);
        }
    }
}