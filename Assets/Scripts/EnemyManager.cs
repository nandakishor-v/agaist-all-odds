using UnityEngine;

public class EnemyManager : MonoBehaviour
{
	[SerializeField] private int maxHealth = 2;
    [SerializeField] private GameObject spriteMask;
    [SerializeField] private float[] alphaCutoffValues;
    [SerializeField] private int points = 5;
	private int currentHealth;
    private int arrayLength;
    private GameSession gameSession;
    void Start()
    {
        currentHealth = maxHealth;
        arrayLength = alphaCutoffValues.Length - 1;
        spriteMask.GetComponent<SpriteMask>().enabled = false;
        gameSession = FindObjectOfType<GameSession>();
    }
    

    public void TakeDamage(int damage)
    {
        Debug.Log("damage taken");
        spriteMask.GetComponent<SpriteMask>().enabled = true;
        currentHealth -= damage;
        EraseEnemy();
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void EraseEnemy()
    {
        spriteMask.GetComponent<SpriteMask>().alphaCutoff = alphaCutoffValues[arrayLength--];
    }

    private void Die()
    {
       gameSession.AddToScore(points);
        Destroy(gameObject);
    }
}
