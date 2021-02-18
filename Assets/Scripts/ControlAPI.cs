using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using SimpleJSON;

public class ControlAPI : MonoBehaviour
{
    public Text namePokemon;
    public Text[] KindPokemon;
    public Text numberPokemon;
    public RawImage imgPokemon;
    public JSONNode pokeInfo;

    private readonly string basePokeURL = "https://pokeapi.co/api/v2/";

    void Start()
    {
        //coloca la imagen negra
       imgPokemon.texture = Texture2D.blackTexture;
        //limpia los textos de name y numero del pokemon
       namePokemon.text = "";
       numberPokemon.text = "";
        //limpia los textos de tipo de pokemon
       foreach (var text in KindPokemon)
       {
           text.text = "";
       }
    }

    public void Button()
    {
        // escoge un numero entre el 1 y 800
        int randomPokemonIndex = Random.Range(1, 808);
        //coloca la imagen negra
        imgPokemon.texture = Texture2D.blackTexture;
        //coloca el numero de pokemos a cargan y el letrero loadind en el nombre
        namePokemon.text = "Loading ...";
        numberPokemon.text = "#" + randomPokemonIndex;
        //limpia los textos de tipo de pokemon
        foreach (var text in KindPokemon)
        {
            text.text = "";
        }
        //inicia la corrutina para obtener el JSON del pokemon seleccionado
        StartCoroutine(GetPokemonAtIndex(randomPokemonIndex));
    }

    IEnumerator GetPokemonAtIndex(int pokemonIndex){
        // armanos la url para el pokemon que escogimos
        string pokemonURL = basePokeURL + "pokemon/" +pokemonIndex.ToString();
        //obtenemos la info de la URL 
        UnityWebRequest pokeInfoRequest = UnityWebRequest.Get(pokemonURL);
        
        yield return pokeInfoRequest.SendWebRequest();

        if (pokeInfoRequest.isNetworkError || pokeInfoRequest.isHttpError)
        {
            Debug.LogError(pokeInfoRequest.error);
            yield break;
        }
        //pasamos el JSON a formato JSONNode
        JSONNode pokeInfo = JSON.Parse(pokeInfoRequest.downloadHandler.text);
        //ordenamos los datos 
        string pokeName = pokeInfo["name"];
        string pokeSpriteAtUrl = pokeInfo["sprites"]["front_default"];
        //ordenamos en otro objeto tipo JSONNode los tipos de ese pokemon
        JSONNode pokeTypes = pokeInfo["types"];
        string[] pokeTypesNames = new string[pokeTypes.Count];
        //guardamos los tipos de pokemos en variable 
        for (int i = 0, j = pokeTypes.Count - 1 ; i < pokeTypes.Count; i++, j--)
        {
            pokeTypesNames[j] = pokeTypes[i]["type"]["name"];
        }
        //obtenemos la textura del pokemon 
        UnityWebRequest pokeSpriteRequest = UnityWebRequestTexture.GetTexture(pokeSpriteAtUrl);

        yield return pokeSpriteRequest.SendWebRequest();

        if (pokeSpriteRequest.isNetworkError || pokeSpriteRequest.isHttpError)
        {
            Debug.LogError(pokeSpriteRequest.error);
            yield break;
        }
        //COLOCAMOS LA IMAGEN DEL POKEMON EN PANTALLA
        imgPokemon.texture = DownloadHandlerTexture.GetContent(pokeSpriteRequest);
        imgPokemon.texture.filterMode = FilterMode.Point;

        namePokemon.text = pokeName;

        for (int i = 0; i < pokeTypesNames.Length; i++)
        {
            KindPokemon[i].text = pokeTypesNames[i];
        }

        UnityWebRequest theBigBandRequest = UnityWebRequest.Get("http://api.tvmaze.com/singlesearch/shows?q=big-bang-theory&embed=episodes");
        
        yield return theBigBandRequest.SendWebRequest();

        if (theBigBandRequest.isNetworkError || theBigBandRequest.isHttpError)
        {
            Debug.LogError(theBigBandRequest.error);
            yield break;
        }
        //pasamos el JSON a formato JSONNode
        JSONNode theBigBandInfo = JSON.Parse(theBigBandRequest.downloadHandler.text);
        //ordenamos los datos 
        print(theBigBandInfo["_embedded"]);

    }
}

