using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnSpriteManager : MonoBehaviour
{
    [SerializeField] private RectTransform zoneSpawnSprite; // Transform de la zone où les sprites seront générés

    [Header("Difficulty rises")]
    [SerializeField] private int roundBatches;
    [SerializeField] private int firstDifficulty;
    [SerializeField] private int secondDifficulty;
    [SerializeField] private int thirdDifficulty;
    [SerializeField] private int fourthDifficulty;

    [Header("ParametersLists")]
    [SerializeField] private List<Color> chooseColors;
    [SerializeField] private List<float> chooseRotations;
    [SerializeField] private List<float> chooseRotationSpeeds;

    [SerializeField] private List<float> chooseScale;




    private List<Color> selectedColors;
    private List<float> selectedRotations;
    private List<Vector3> selectedMovements;
    private List<float> selectedScale;
    private List<float> selectedRotationSpeeds;


    private Color selectedTargetColor;
    private float selectedTargetRotation;

    private void Awake()
    {
        chooseColors = new List<Color> { Color.red, Color.blue, Color.green, Color.magenta};
        chooseRotations = new List<float> { 0, 180, 90, 270, 45, 315, 225, 135 };
        chooseScale = new List<float> { 1.3f, 0.8f, 1.6f, 0.6f, 1.9f, 0.4f, 2.2f };
        chooseRotationSpeeds = new List<float> { 45f, -45f, 90f, -90f, 135f, -135f, 180f, -180f, 225f, -225f, 270f, -270f, 315f, -315f };

    }

    
    public void ChooseTransformation(int score)
    {
        if (score==0 || roundBatches==0 || score % roundBatches == 0)
        {
            selectedColors = new List<Color> { Color.white};
            selectedRotations = new List<float> { 0f };
            selectedScale = new List<float> { 1f };
            selectedRotationSpeeds = new List<float> { 0f };

            selectedMovements = new List<Vector3> { Vector3.zero };
            selectedTargetColor = Color.white;
            selectedTargetRotation = 0f;



            //Penser à rajouter du random sur le nombre de fonctions à exécuter
            int numberOfFunctionsToExecute = 0;
            if(score > fourthDifficulty)
            {
                numberOfFunctionsToExecute = Random.Range(0, 5);
            }
            else if (score > thirdDifficulty)
            {
                numberOfFunctionsToExecute = Random.Range(0, 4);
            }
            else if (score > secondDifficulty)
            {
                numberOfFunctionsToExecute = Random.Range(0, 3);
            }
            else if (score > firstDifficulty)
            {
                numberOfFunctionsToExecute = Random.Range(0, 2);
            }

            // Coefficients de pondération pour chaque fonction
            List<float> weights = new() { 4f, 2f, 3f, 3f, 1f };


            List<int> selectedIndices = new();
            List<float> cumulativeWeights = new();
            float totalWeight = 0f;

            // Calculer les poids cumulatifs
            foreach (float weight in weights)
            {
                totalWeight += weight;
                cumulativeWeights.Add(totalWeight);
            }

            while (selectedIndices.Count < numberOfFunctionsToExecute)
            {
                float randomValue = Random.Range(0f, totalWeight);
                for (int i = 0; i < cumulativeWeights.Count; i++)
                {
                    if (randomValue < cumulativeWeights[i])
                    {
                        if (!selectedIndices.Contains(i))
                        {
                            selectedIndices.Add(i);
                        }
                        break;
                    }
                }
            }

            // Exécuter les fonctions sélectionnées
            foreach (int index in selectedIndices)
            {
                switch (index)
                {
                    case 0:
                        ChooseColors(score);
                        break;
                    case 1:
                        ChooseRotations(score);
                        break;
                    case 2:
                        ChooseMovements(score);
                        break;
                    case 3:
                        ChooseScale(score);
                        break;
                    case 4:
                        HowManySpriteToFind(score);
                        break;
                }
            }
        }
    }

    private void HowManySpriteToFind(int score)
    {
        int numberToFind = 1 + score / 50;
        if(numberToFind > 1)
        {
            numberToFind = Random.Range(2, numberToFind + 1);
        }
        GameManager.Instance.howManySpriteToFind = numberToFind < 5 ? numberToFind : 4;
    }

    public void ChooseColors(int score)
    {
        int numberOfColors = Mathf.Clamp(1+ score / 20, 0, chooseColors.Count);
        numberOfColors = Random.Range(1, numberOfColors + 1);
        selectedColors = new List<Color>();
        for (int i = 0; i < numberOfColors; i++)
        {
            Color randomColor;
            do
            {
                randomColor = chooseColors[Random.Range(0, chooseColors.Count)];
            } while (selectedColors.Contains(randomColor));
            selectedColors.Add(randomColor);
        }

        selectedTargetColor = selectedColors[Random.Range(0, selectedColors.Count)];
        
    }

    public void ChooseRotations(int score)
    {
        int numberOfRotations = Mathf.Clamp(1+ score / 15, 0, 1 + chooseRotations.Count/2);
        selectedRotations = new List<float>();
        if(numberOfRotations == 1 + chooseRotations.Count / 2)
        {
            //Générer 8 valeurs d'angles aléatoires
            for (int i = 0; i < 8; i++)
            {
                float randomRotation;
                do
                {
                    randomRotation = Random.Range(0, 360);
                } while (selectedRotations.Contains(randomRotation));
                selectedRotations.Add(randomRotation);
            }
        }
        else
        {
            for (int i = 0; i < numberOfRotations*2; i++)
            {
                selectedRotations.Add(chooseRotations[i]);
            }
        }
        

        if (Random.value < GenerateExponentialRandom(15f / score))
        {
            selectedTargetRotation = 0f;
        }
        else
        {
            selectedTargetRotation = selectedRotations[Random.Range(0, selectedRotations.Count)];
        }
        if(Random.value < GenerateExponentialRandom(30f / score))
        {
            ChooseRotationSpeed(score);
        }
        
    }

    private void ChooseRotationSpeed(int score)
    {
        int numberOfRotationSpeeds = Mathf.Clamp(1 + score / 15, 0, chooseRotationSpeeds.Count);
        selectedRotationSpeeds = new List<float>();
        for(int i = 0; i < numberOfRotationSpeeds; i++)
        {
            selectedRotationSpeeds.Add(chooseRotationSpeeds[i]);
        }
        numberOfRotationSpeeds = Random.Range(1, numberOfRotationSpeeds + 1);
        for (int i = 0; i < selectedRotationSpeeds.Count - numberOfRotationSpeeds; i++)
        {
            selectedRotationSpeeds.Remove(selectedRotationSpeeds[Random.Range(0, selectedRotationSpeeds.Count)]);
        }
    }

    public void ChooseMovements(int score)
    {
        int numberOfRotationSpeeds = Mathf.Clamp(1 + score / 20, 0, chooseRotationSpeeds.Count);

        //Générer numberOfRotationSpeeds valeurs de mouvement uniformément réparties sur un cercle
        selectedMovements = new List<Vector3>();
        for (int i = 0; i < numberOfRotationSpeeds; i++)
        {
            float angle = 360f / numberOfRotationSpeeds * i;
            selectedMovements.Add(new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0));
        }

    }

    public void ChooseScale(int score)
    {
        int numberOfScales = Mathf.Clamp(1 + score / 15, 0, chooseScale.Count);
        selectedScale = new List<float>();
        for (int i = 0; i < numberOfScales; i++)
        {
            selectedScale.Add(chooseScale[i]);
        }
        numberOfScales = Random.Range(1, numberOfScales + 1);
        for (int i = 0; i < selectedScale.Count - numberOfScales; i++)
        {
            selectedScale.Remove(selectedScale[Random.Range(0, selectedScale.Count)]);
        }
    }


    private Color GetRandomColor()
    {
        return selectedColors[Random.Range(0, selectedColors.Count)];
    }

    private float GetRandomRotation()
    {
        return selectedRotations[Random.Range(0, selectedRotations.Count)];
    }

    private Vector3 GetRandomMovement()
    {
        return selectedMovements[Random.Range(0, selectedMovements.Count)];
    }

    private float GetRandomScale()
    {
        return selectedScale[Random.Range(0, selectedScale.Count)];
    }
    private float GetRandomRotationSpeed()
    {
        return selectedRotationSpeeds[Random.Range(0, selectedRotationSpeeds.Count)];
    }

    public void ApplyTransformations(GameObject spriteObject, bool isTarget, int spriteIndex, int score) {
        spriteObject.transform.SetParent(zoneSpawnSprite);
        spriteObject.GetComponent<RectTransform>().localScale = Vector3.one;

        // Positionner les sprites dans la zone
        float x = Random.Range(zoneSpawnSprite.rect.xMin, zoneSpawnSprite.rect.xMax);
        float y = Random.Range(zoneSpawnSprite.rect.yMin, zoneSpawnSprite.rect.yMax);
        spriteObject.GetComponent<RectTransform>().localPosition = new Vector3(x, y, 0);

        //1/2 chance de rebondir sur les bords
        bool isBouncing = false;
        if (Random.Range(0, 2) == 0)
        {
            isBouncing = true;
        }
        
        bool isSinusoidal = false;
        if (Random.Range(0, 2) == 0)
        {
            isSinusoidal = true;
        }

        bool sameSpeed = false;
        float speed = Random.Range(50, 51+score*4);
        if (Random.Range(0, 2) == 0)
        {
            sameSpeed = true;
        }

        bool sameSinusoidal = false;
        float amplitude = Random.Range(2, 2.1f + score / 4f);
        float frequency = Random.Range(1f, 1.1f + score / 6f);
        if (Random.Range(0, 2) == 0)
        {
            sameSinusoidal = true;
        }

        if (isTarget)
        {
            spriteObject.GetComponent<Image>().color = selectedTargetColor;
            
            spriteObject.transform.Rotate(Vector3.forward, selectedTargetRotation);
            
            spriteObject.GetComponent<SpriteClickable>().Initialized(
                spriteIndex, 
                GetRandomMovement(),
                sameSpeed ? speed : Random.Range(50, 51 + score * 2), 
                isBouncing, 
                isSinusoidal, 
                sameSinusoidal ? amplitude : Random.Range(2, 2.1f + score / 6f),
                sameSinusoidal ? frequency : Random.Range(2f, 2.1f + score / 4f),
                GetRandomRotationSpeed()
            );
            float scale = GetRandomScale();
            spriteObject.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, 1);
            
        }
        else
        {
            spriteObject.GetComponent<Image>().color = GetRandomColor();
            
            spriteObject.GetComponent<RectTransform>().localRotation *= Quaternion.Euler(0, 0, GetRandomRotation());
            
            spriteObject.GetComponent<SpriteClickable>().Initialized(
                spriteIndex, 
                GetRandomMovement(),
                sameSpeed ? speed : Random.Range(50, 51 + score * 2),
                isBouncing, 
                isSinusoidal,
                sameSinusoidal ? amplitude : Random.Range(2, 2.1f + score / 6f),
                sameSinusoidal ? frequency : Random.Range(2f, 2.1f + score / 4f),
                GetRandomRotationSpeed()
            );
            float scale = GetRandomScale();
            spriteObject.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, 1);
            
        }


    }

    float GenerateExponentialRandom(float lambda)
    {
        float u = Random.value; // Génère un nombre aléatoire uniforme entre 0 et 1
        float result = Mathf.Exp(-u / lambda); // Appliquer ta fonction avec la nouvelle abscisse
        return result;
    }


}
