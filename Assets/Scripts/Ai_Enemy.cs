using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ai_Enemy : MonoBehaviour
{
    public int maxHealth = 100;
    public int speed = 1;
    public float attackRate = 2f;
    public bool IsAttack = false;
    public int currentHealth;

    private int _damage = 10;
    private float _distance;
    private float _lookDistance = 5f;
    private float _attackRange = 0.5f;
    private bool _isFacingRight = true;

    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private GameObject _player;
    [SerializeField] private Animator _anim;
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private Player _Player;

    void Start()
    {
        currentHealth = maxHealth;
    }
    private void Update()
    {
        _distance = Vector2.Distance(transform.position, _player.transform.position);
        Vector2 direction = _player.transform.position - transform.forward;
        transform.position = Vector2.MoveTowards(this.transform.position, _player.transform.position, speed * Time.deltaTime);
        _anim.SetFloat("Speed", Mathf.Abs(speed));

        if (_distance <= _lookDistance)
        {
            speed = 1;
        }
        else if(_distance < 0.5)
        {
            speed = 0;
        }
        else
        {
            speed = 0;
        }
        Flip();
    }
    private void Flip()
    {
        if (_isFacingRight && _distance < 0f || !_isFacingRight && _distance > 0f)
        {
            _isFacingRight = !_isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
    }
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        _anim.SetTrigger("Hurt");

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        _anim.SetBool("IsDead", true);

        GetComponent<Collider2D>().enabled = false;
        this.enabled = false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(_attackPoint.position, _attackRange);
    }
    private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "PlayersMelee")
        {
            _anim.SetFloat("Speed", 0);
            _anim.SetBool("IsAttack", true);
            speed = 0;
            StartCoroutine(Attack()); 
            IsAttack = true;
        }
        
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "PlayersMelee")
        {
            _anim.SetBool("IsAttack", false);
            _anim.SetFloat("Speed", 0);
            speed = 1;
            StopCoroutine(Attack());
            IsAttack = false;
        }
    }
    private IEnumerator Attack()
    {
        while (true)
        {
            _Player.TakeDamage(_damage);
            yield return new WaitForSeconds(attackRate);
        }
    }
}

