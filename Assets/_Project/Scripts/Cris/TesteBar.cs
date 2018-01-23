
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class TesteBar : MonoBehaviour
{

    public List<InteractableObject> references;
    public Image prefabLeft;
    public Image prefabRight;
    public Color colorLeft;
    public Color colorRight;

    private List<ObjetoInstanciar> listaObjetos = new List<ObjetoInstanciar>();
    [SerializeField, HideInInspector]
    private InteractableBluePrint[] listaTotal;
    private float maxEnergyValue;
    private float totalHeight;
    private RectTransform screenDimension;

    private void Awake()
    {
        listaTotal = Resources.LoadAll<InteractableBluePrint>("InteractableObjects");
        maxEnergyValue = GetEnergyMaxValue();
        screenDimension = GetComponent<RectTransform>();
        screenDimension.sizeDelta = new Vector2(screenDimension.rect.width, (Screen.height * 1.15f));
    }

    private void Start()
    {
        totalHeight = screenDimension.rect.height;
    }

    private float GetEnergyMaxValue()
    {
        float maxValue = 0f;
        for (int i = 0; i < listaTotal.Length; i++)
        {
            maxValue += listaTotal[i].kwh;
        }
        return maxValue;
    }

    public void InsertOnList(InteractableObject objeto)
    {
        references.Add(objeto);
        AdicionaBarra(objeto);
        OrganizeList();
    }


    public void RemoveFromList(InteractableObject objeto)
    {
        references.Remove(objeto);
        RemoverBarra(objeto);
    }

    private void AdicionaBarra(InteractableObject objeto)
    {
        float percent = GetFillPercent(objeto.interactableBluePrint.kwh);
        Image prefab;
        Color colorBar;
        if ((listaObjetos.Count % 2) == 0)
        {
            prefab = prefabRight;
            colorBar = colorRight;
        }
        else
        {
            prefab = prefabLeft;
            colorBar = colorLeft;
        }
        Image imagem = Instantiate(prefab, transform);
        IndividualEnergyViewer individualEnergyViewer = imagem.GetComponent<IndividualEnergyViewer>();
        individualEnergyViewer.energy = objeto.interactableBluePrint.energy;
        individualEnergyViewer.enabled = true;
        imagem.color = colorBar;
        imagem.GetComponentsInChildren<Image>()[1].sprite = objeto.interactableBluePrint.icon;
        //imagem.GetComponentInChildren<Text>().text = objeto.interactableBluePrint.kwh.ToString("0.00");
        imagem.rectTransform.sizeDelta = new Vector2(28f, percent);
        listaObjetos.Add(new ObjetoInstanciar() { interactableObject = objeto, imagePrefab = imagem });
        //OrganizeList();
    }

    private void OrganizeList()
    {

        List<ObjetoInstanciar> listaObjetosTemp = new List<ObjetoInstanciar>();
        listaObjetosTemp = listaObjetos.OrderByDescending(x => x.interactableObject.interactableBluePrint.kwh).ToList();
        listaObjetos = new List<ObjetoInstanciar>();
        int lastIndex = listaObjetosTemp.Count - 1;
        int firstIndex = 0;

        for (int i = 0; i < listaObjetosTemp.Count; i++)
        {
            if ((i % 2) == 0)
            {
                Destroy(listaObjetosTemp[firstIndex].imagePrefab.gameObject);
                AdicionaBarra(listaObjetosTemp[firstIndex].interactableObject);
                firstIndex += 1;
            }
            else
            {
                Destroy(listaObjetosTemp[lastIndex].imagePrefab.gameObject);
                AdicionaBarra(listaObjetosTemp[lastIndex].interactableObject);
                lastIndex -= 1;
            }

        }

        //List<ObjetoInstanciar> listaObjetosTemp = new List<ObjetoInstanciar>();
        //listaObjetosTemp = listaObjetos.OrderByDescending(x => x.interactableObject.interactableBluePrint.kwh).ToList();
        //listaObjetos = new List<ObjetoInstanciar>();
        //int lastIndex = listaObjetosTemp.Count - 1;
        //int firstIndex = 0;
        //for (int i = 0; i < listaObjetosTemp.Count; i++)
        //{
        //    if ((i % 2) == 0)
        //    {
        //        listaObjetos.Add(listaObjetosTemp[firstIndex]);
        //        firstIndex += 1;
        //    }
        //    else
        //    {
        //        listaObjetos.Add(listaObjetosTemp[lastIndex]);
        //        lastIndex -= 1;
        //    }
        //}
    }

    private float GetFillPercent(float value)
    {
        float percentValue = (value * totalHeight / maxEnergyValue);
        return percentValue;
    }

    private void RemoverBarra(InteractableObject objeto)
    {
        float percent = GetFillPercent(objeto.interactableBluePrint.kwh);
        ObjetoInstanciar objetoInstanciar = new ObjetoInstanciar();
        foreach (var item in listaObjetos)
        {
            if (item.interactableObject.Equals(objeto))
            {
                Destroy(item.imagePrefab.gameObject);
                objetoInstanciar = item;
            }
        }
        listaObjetos.Remove(objetoInstanciar);
        OrganizeList();
        //List<ObjetoInstanciar> listaObjetosTemp = new List<ObjetoInstanciar>();
        //listaObjetosTemp = listaObjetos.OrderByDescending(x=>x.interactableObject.interactableBluePrint.kwh).ToList();
        //listaObjetos = new List<ObjetoInstanciar>();
        //int lastIndex = listaObjetosTemp.Count - 1;
        //int firstIndex = 0;

        //for (int i = 0; i < listaObjetosTemp.Count; i++)
        //{
        //    if ((i % 2) == 0)
        //    {
        //        Destroy(listaObjetosTemp[firstIndex].imagePrefab.gameObject);
        //        AdicionaBarra(listaObjetosTemp[firstIndex].interactableObject);
        //        firstIndex += 1;
        //    }
        //    else
        //    {
        //        Destroy(listaObjetosTemp[lastIndex].imagePrefab.gameObject);
        //        AdicionaBarra(listaObjetosTemp[lastIndex].interactableObject);
        //        lastIndex -= 1;
        //    }

        //}

        //foreach (var item in listaObjetosTemp)
        //{
        //    Destroy(item.imagePrefab.gameObject);
        //    AdicionaBarra(item.interactableObject);
        //}
    }
    
}

public class ObjetoInstanciar
{
    public InteractableObject interactableObject { get; set; }
    public Image imagePrefab { get; set; }
}
