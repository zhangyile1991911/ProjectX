using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
public class FryEnter : MonoBehaviour
{
    public Button startButton;
    public FryModule module;

    public Recipe recipe;
    // Start is called before the first frame update
    void Start()
    {
        startButton.OnClickAsObservable().Subscribe(_ =>
        {
            module.SetFryRecipe(recipe);
            module.StartFry();
            startButton.gameObject.SetActive(false);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
