using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;

public class GameManager : MonoBehaviour
{
    public static GameManager gameManagerInstance;

    public GameObject shipPrefab;
    public Text scoreText;

    private int currentScore;
    private Entity shipEntityPrefab;
    private EntityManager entityManager;
    private BlobAssetStore blobAssetStore;

    void Awake()
    {
        if (gameManagerInstance != null && gameManagerInstance != this)
        {
            Destroy(gameObject);
            return;
        }

        gameManagerInstance = this;

        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        blobAssetStore = new BlobAssetStore();
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(World.DefaultGameObjectInjectionWorld, blobAssetStore);

        shipEntityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(shipPrefab, settings);
    }

    private void Start()
    {
        currentScore = 0;
        DisplayScore();
        SpawnShip();
    }

    private void DisplayScore()
    {
        scoreText.text = "Score: " + currentScore;
    }

    void SpawnShip()
    {
        Entity newShip = entityManager.Instantiate(shipEntityPrefab);

        Translation shipTranslation = new Translation()
        {
            Value = new float3(0, 0, 0)
        };

        entityManager.AddComponentData(newShip, shipTranslation);
    }

    private void OnDestroy()
    {
        blobAssetStore.Dispose();
    }

}
