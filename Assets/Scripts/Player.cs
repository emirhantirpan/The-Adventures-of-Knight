using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int attackDamage = 2;
    public float attackRate = 0.5f;
    public int maxHealth = 100;
    
    private float _nextAttackTime = 0f;
    private float _horizontal;
    private float _speed = 6f;
    private float _jumpPower = 10f;
    private bool _isFacingRight = true;
    private bool _canRoll = true;
    private bool _isRolling = false;
    private float _rollPower = 8f;
    private float _rollingTime = 0.6111112f;
    private float _rollingCooldown = 0.3f;
    private float _attackRange = 0.5f;
    private int _currentHealth;
    private int _takeDamage = 10;

    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private Animator _anim;
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private LayerMask _enemyLayers;
    [SerializeField] private Ai_Enemy _ai;

    private void Start()
    {
        _currentHealth = maxHealth;

        Invoke("Die", 5f);
    }
    private void Update()
    {
        if (_isRolling)
        {
            return;
        }

        _horizontal = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump") && IsGrounded())
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _jumpPower);
        }

        if (Input.GetButtonUp("Jump") && _rb.velocity.y > 0f)
        {
            _rb.velocity = new Vector2(_rb.velocity.x, _rb.velocity.y * 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.LeftShift) && _canRoll)
        {
            StartCoroutine(Roll());
        }
            if (_isRolling == true)
            {
                _anim.SetBool("Roll", true);
            }
            else
            {
                _anim.SetBool("Roll", false);
            }

        if (Time.time >= _nextAttackTime)
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                Attack();
                _nextAttackTime = Time.time + 1f / attackRate;
            }
        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            Block();
        }

        Flip();
    }
    private void FixedUpdate()
    {
        if (_isRolling)
        {
            return;
        }

        _rb.velocity = new Vector2(_horizontal * _speed, _rb.velocity.y);
    }
    private bool IsGrounded()
    {
        return Physics2D.OverlapCircle(_groundCheck.position, 0.2f, _groundLayer);
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            _anim.SetBool("Jump", false);
        }
    }
    private void OnCollisionExit2D(Collision2D col)
    {
        if (col.gameObject.tag == "Ground")
        {
            _anim.SetBool("Jump", true);
        }
    }
    private void Flip()
    {
        if (_isFacingRight && _horizontal < 0f || !_isFacingRight && _horizontal > 0f)
        {
            _isFacingRight = !_isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;
        }
        _anim.SetFloat("Speed", Mathf.Abs(_horizontal));
    }
    private IEnumerator Roll()
    {
        _canRoll = false;
        _isRolling = true;
        _rb.velocity = new Vector2(transform.localScale.x * _rollPower, 0f);
        yield return new WaitForSeconds(_rollingTime);
        _isRolling = false;
        yield return new WaitForSeconds(_rollingCooldown);
        _canRoll = true;
    }
    private void Attack()
    {
        _anim.SetTrigger("Attack");

        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(_attackPoint.position, _attackRange, _enemyLayers);

        foreach(Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<Ai_Enemy>().TakeDamage(attackDamage);
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (_attackPoint == null)
        {
            return;
        }

        Gizmos.DrawWireSphere(_attackPoint.position, _attackRange);
    }
    /*private void OnTriggerEnter2D(Collider2D col)
    {
        if(col.gameObject.tag == "BanditsMelee")
        {
            _anim.SetBool("IsHurt", true);
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.tag == "BanditsMelee")
        {
            _anim.SetBool("IsHurt", false);
        }
    }*/
    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            _anim.SetBool("IsDead", true);
            Die();
        }
    }
    public void Block()
    {
        _anim.SetTrigger("Block");
        if (_ai.IsAttack == true)
        {
            _anim.SetTrigger("Block");
            _ai.currentHealth -= 10;
        }
    }
    private void Die()
    {
        _anim.SetBool("IsDead", true);
        //Time.timeScale = 0;
    }
    
}
