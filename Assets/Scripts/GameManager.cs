using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class GameManager : MonoBehaviour
{
    List<Unit> Units;
    HashSet<string> UnitName;
    List<Player> players;
    [SerializeField] GameObject FunctionBar;
    [SerializeField] GameObject MiniMap;
    GameObject UIBar;
    List<GameObject> currentUnits;
    Dictionary<string, int> dictionary;
    Dictionary<string, int> eDictionary;
    //Dictionary<string, int> aDictionary;
    Dictionary<string, int> cDictionary;
    Unit[] AllUnits;
    List<List<Equipment>>[] AllEquipments;
    //List<List<AbilityData>>[] AllAbilities;
    List<Building>[] AllConstructions;
    UnitSelection unitselection;
    GameObject current;

    // Start is called before the first frame update
    void Awake()
    {
        Units = new List<Unit>();
        UnitName = new HashSet<string>();
        players = new List<Player>(Data.playerNumber);
        for (int i = 0; i < players.Capacity; i++)
            players.Add(new Player(generatePlayerID(), i));
        SaveSystem.SavePlayerList(players);
        if (FunctionBar.transform.childCount != 0)
            UIBar = FunctionBar.transform.GetChild(0).gameObject;
        else
            UIBar = null;
        currentUnits = null;
        AllUnits = SaveSystem.DecryptUnit();
        dictionary = SaveSystem.DecryptDictionary("UnitDict.rts");
        eDictionary = SaveSystem.DecryptDictionary("EquipmentsDict.rts");
        AllEquipments = SaveSystem.DecryptEquipment();
        AllConstructions = SaveSystem.DecryptConstructions();
        cDictionary = SaveSystem.DecryptDictionary("ConstructionDict.rts");
        unitselection = GameObject.Find("Main Camera").GetComponent<UnitSelection>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
            GameObject.Find("UI").transform.GetChild(2).GetChild(0).Find("SelectionBar").gameObject.SetActive(false);
        if (UIBar != null && UIBar.name.Equals("DetailBar(Clone)") && current != null)
        {
            Unit c_unit = current.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit();
            UIBar.transform.Find("HP").GetComponent<TextMeshProUGUI>().text = c_unit.getHitPoint() + " / " + c_unit.getMaxHitPoint();
        }

        if(UIBar != null && UIBar.name.Equals("ProductionBar(Clone)") && current != null)
        {
            ConstructionFunction cf = current.GetComponent<ConstructionFunction>();
            LinkedList<string> queue = cf.getProductionQueue();
            int[] count = new int[cf.getProductionUnitCount()];
            foreach(string str in queue)
                count[cf.getIndex(str)]++;
            BarInfo info = UIBar.GetComponent<BarInfo>();
            Transform t;
            for(int i = 0; i < count.Length; i++)
            {
                t = UIBar.transform.GetChild(info.getIndex("Production") + i).GetChild(0).GetChild(1);
                if (count[i] == 0)
                {
                    t.GetChild(0).gameObject.SetActive(false);
                    t.GetChild(1).gameObject.SetActive(false);
                    t.GetChild(2).gameObject.SetActive(false);
                    continue;
                }
                t.GetChild(0).gameObject.SetActive(true);
                t.GetChild(1).gameObject.SetActive(true);
                t.GetChild(1).GetComponent<TextMeshProUGUI>().text = "" + count[i];
                t.GetChild(2).gameObject.SetActive(true);
                t.GetChild(2).GetComponent<TextMeshProUGUI>().text = (int)(cf.getProgress() * 100) + "%";
            }
        }

        if(UIBar != null && UIBar.name.Equals("ConstructingBar(Clone)") && current != null)
        {
            Construction c = current.GetComponent<Construction>();
            Unit u = current.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit();
            UIBar.transform.GetChild(UIBar.GetComponent<BarInfo>().getIndex("Progress")).GetComponent<Slider>().value = (Time.time - c.getStartTime()) / u.getProduceTime();
            if ((Time.time - c.getStartTime()) > u.getProduceTime())
            {
                if (current.GetComponent<ConstructionFunction>() != null)
                    SetUpProductionBar(current.transform.GetChild(0).name, 0);
                else
                    SetUpUnitBar(current.transform.GetChild(0).name, 0);
            }
        }
            
        /*if (Input.GetKeyUp(KeyCode.A))
            GameObject.Find("Target").GetComponent<Target>().setAMove(true);
        if (Input.GetMouseButtonUp(1))
            GameObject.Find("Target").GetComponent<Target>().setAMove(false);*/

        //if (Input.GetMouseButtonUp(1))
          //  current = null;
        //detectNewUnit();
    }

    public void setCurrentUnit(List<GameObject> units)
    {
        currentUnits = new List<GameObject>();
        for (int i = 0; i < units.Count; i++)
            currentUnits.Add(units[i]);
    }

    public void setCurrentUnitEquipment(Equipment equipment)
    {
        if (currentUnits.Count == 0)
            return;
        List<GameObject> toRemove = new List<GameObject>();
        foreach(GameObject gameobj in currentUnits)
        {
            try
            {
                gameobj.transform.GetChild(0).GetComponent<UnitLoad>().addEquipment(equipment);
            }
            catch(MissingReferenceException)
            {
                toRemove.Add(gameobj);
                continue;
            }
            
        }
        for (int i = 0; i < toRemove.Count; i++)
            currentUnits.Remove(toRemove[i]);

        UIBar.transform.Find("SelectionBar").gameObject.SetActive(false);

        refreshAbility();
    }

    public void setConstructionBar()
    {
        if (UIBar != null)
            Destroy(UIBar);
        GameObject obj = Resources.Load("ConstructionBar") as GameObject;
        UIBar = Instantiate(obj, FunctionBar.transform);
        GameObject button;
        for(int i = 0; i < AllConstructions[0].Count; i++)
        {
            button = Resources.Load(AllConstructions[0][i].getName() + "ConstructionButton") as GameObject;
            Instantiate(button, UIBar.transform.GetChild(i));
        }
    }

    public void construct(string name)
    {
        int index = cDictionary[name];
        int findex = index / 10;
        int uindex = index % 10;
        gameObject.GetComponent<ConstructionActive>().set(AllConstructions[findex][uindex]);
    }

    public Building getConstruction(string name)
    {
        int index = cDictionary[name];
        int findex = index / 10;
        int uindex = index % 10;
        return AllConstructions[findex][uindex];
    }

    public float getUnitHeight(string name)
    {
        return AllUnits[dictionary[name]].getHeight();
    }    

    public List<Player> getPlayers()
    {
        return players;
    }

    public void addNewUnit(Unit unit)
    {
        Units.Add(unit);
        UnitName.Add("Unit" + unit.getID());
        SaveSystem.SaveUnit(Units);
    }

    public Unit getNewUnit(string name)
    {
        return AllUnits[dictionary[name]];
    }

    public Equipment getNewEquipment(string name)
    {
        int equip = eDictionary[name];
        int equipmentIndex = equip % 10;
        equip /= 10;
        int slotIndex = equip % 10;
        equip /= 10;
        int unitIndex = equip;
        
        return AllEquipments[unitIndex][slotIndex][equipmentIndex];
    }

    public GameObject getMiniMap()
    {
        return MiniMap;
    }

    public List<Unit> getUnitList()
    {
        return Units;
    }

    public void removeUnit(Unit unit)
    {
        for (int i = 0; i < Units.Count; i++)
        {
            if (Units[i].getID() == unit.getID())
            {
                Units.RemoveAt(i);
                UnitName.Remove("Unit" + unit.getID());
                break;
            }
        }
            
    }

    public bool IsUnitExists(string name)
    {
        return UnitName.Contains(name);
    }

    public void SetUpSelectedBar()
    {
        if(UIBar != null)
            Destroy(UIBar);
        List<GameObject> selected = unitselection.getCurrentSelected();
        selected = selected.OrderBy(o => o.transform.GetChild(0).name).ToList();
        GameObject gameobj = Resources.Load("GroupSelect") as GameObject;
        UIBar = Instantiate(gameobj, FunctionBar.transform);
        UIBar.GetComponent<GroupSelectScript>().displaySelected(selected);

    }

    public void refreshAbility()
    {
        if (!UIBar.name.Equals("DetailBar(Clone)"))
            return;
        Transform tr;
        for (int i = 1; i <= 4; i++)
        {
            tr = UIBar.transform.Find("AbilitySlot" + i);
            if (tr.childCount > 0)
                Destroy(tr.GetChild(0).gameObject);
        }
        GameObject gameobj;
        List<GameObject> currentSelected = unitselection.getCurrentSelected();
        Unit selectedUnit = current.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit();
        List<AbilityData> a = selectedUnit.getAbility();
        /*Attack attack = current.transform.GetChild(3).GetComponent<Attack>();
        for (int i = 0; i < a.Count; i++)
        {
            gameobj = Resources.Load(a[i].getName() + "Button") as GameObject;
            Instantiate(gameobj, UIBar.transform.Find("AbilitySlot" + (i + 1)));
        }*/

        BarInfo info = UIBar.GetComponent<BarInfo>();
        current.GetComponent<AbilityLoad>().updateAblity();

        for (int i = 0; i < a.Count; i++)
        {
            gameobj = Instantiate(Resources.Load("AbilityButton"), UIBar.transform.GetChild(info.getIndex("Ability") + i)) as GameObject;
            gameobj.GetComponent<AbilityButton>().SetLoad(current.GetComponent<AbilityLoad>(), i);
            gameobj.GetComponent<AbilityActive>().setIndex(i);
        }
    }

    public void SetUpUnitBar(string name, int index)
    {
        current = currentUnits[index];
        //clear current UI Bar if it is not null
        if(UIBar != null)
            Destroy(UIBar);

        if (current.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().isBuilding() && getConstruction(name).getBuildingType() == 1)
        {
            SetUpProductionBar(name, index);
            return;
        }

        if(current.GetComponent<Animator>() != null && current.GetComponent<Animator>().enabled)
        {
            SetUpConstructingBar(name, index);
            return;
        }

        //Instantiate a blank Detail Bar
        List<GameObject> currentSelected = unitselection.getCurrentSelected();
        GameObject gameobj = Resources.Load("DetailBar") as GameObject;
        UIBar = Instantiate(gameobj, FunctionBar.transform);
        //Load Unit Image
        GameObject picture = new GameObject("Picture");
        picture.AddComponent<RawImage>();
        Texture texture = Resources.Load(name + "Image") as Texture;
        picture.GetComponent<RawImage>().texture = texture;
        RectTransform pictureTransform = picture.GetComponent<RectTransform>();
        pictureTransform.sizeDelta = UIBar.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
        pictureTransform.position = UIBar.transform.GetChild(0).GetComponent<RectTransform>().position;
        picture.transform.SetParent(UIBar.transform.GetChild(0));
        pictureTransform.localScale = new Vector3(1, 1, 1);
        //Load Unit Name
        string display_name = "";
        bool all_upper_case = true;
        for(int i = 0; i < name.Length; i++)
        {
            if (name[i] >= 'A' && name[i] <= 'Z' && i != 0)
            {
                display_name += ' ';
                display_name += name[i];
            }
            else
                display_name += name[i];
            if (name[i] >= 'a' && name[i] <= 'z')
                all_upper_case = false;
        }
        if (all_upper_case)
            display_name = name;
        UIBar.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(display_name);
        //Load Equipment Slots
        List<List<Equipment>> list = AllEquipments[dictionary[name]];
        BarInfo info = UIBar.GetComponent<BarInfo>();
        info.Set();
        for(int i = 0; i < list.Count; i++)
        {
            //gameobj = Resources.Load(name + "EquipmentSlot" + (i + 1)) as GameObject;
            gameobj = Resources.Load("EquipmentCallButton") as GameObject;
            GameObject button = Instantiate(gameobj, UIBar.transform.GetChild(info.getIndex("Equipment") + i));
            button.GetComponent<EquipmentCallButton>().set(name, i);
        }
        Unit selectedUnit = currentSelected[index].transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit();


        List<AbilityData> a = selectedUnit.getAbility();
        /*Attack attack = currentSelected[index].transform.GetChild(3).GetComponent<Attack>();
        for(int i = 0; i < a.Count; i++)
        {
            gameobj = Resources.Load(a[i].getName() + "Button") as GameObject;
            Instantiate(gameobj, UIBar.transform.GetChild(info.getIndex("Ability") + i));
        }*/

        for(int i = 0; i < a.Count; i++)
        {
            gameobj = Instantiate(Resources.Load("AbilityButton"), UIBar.transform.GetChild(info.getIndex("Ability") + i)) as GameObject;
            gameobj.GetComponent<AbilityButton>().SetLoad(currentSelected[index].GetComponent<AbilityLoad>(), i);
            gameobj.GetComponent<AbilityActive>().setIndex(i);
        }

        if (selectedUnit.getTroop() > 0)
        {
            UIBar.transform.Find("Out").gameObject.SetActive(true);
            UIBar.transform.Find("AllOut").gameObject.SetActive(true);
        }
    }

    public void SetUpTroopRemovalBar()
    {
        if (UIBar == null)
            return;
        Transform select = UIBar.transform.Find("SelectionBar");
        select.gameObject.SetActive(true);
        UnitLoad load = current.transform.GetChild(0).GetComponent<UnitLoad>();
        int index = 0;
        foreach(GameObject troop in load.getTroops())
        {
            if (troop == null)
                continue;
            Instantiate(Resources.Load(troop.transform.GetChild(0).name + "TroopRemoveButton") as GameObject, select.GetChild(index));
            index++;
        }
    }    

    public void SetUpProductionBar(string name, int index)
    {
        current = currentUnits[index];
        if (UIBar != null)
            Destroy(UIBar);

        if(current.GetComponent<Animator>().enabled)
        {
            SetUpConstructingBar(name, index);
            return;
        }

        //Instantiate a blank Detail Bar
        List<GameObject> currentSelected = unitselection.getCurrentSelected();
        GameObject gameobj = Resources.Load("ProductionBar") as GameObject;
        UIBar = Instantiate(gameobj, FunctionBar.transform);
        //Load Unit Image
        GameObject picture = new GameObject("Picture");
        picture.AddComponent<RawImage>();
        Texture texture = Resources.Load(name + "Image") as Texture;
        picture.GetComponent<RawImage>().texture = texture;
        RectTransform pictureTransform = picture.GetComponent<RectTransform>();
        pictureTransform.sizeDelta = UIBar.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
        pictureTransform.position = UIBar.transform.GetChild(0).GetComponent<RectTransform>().position;
        picture.transform.SetParent(UIBar.transform.GetChild(0));
        pictureTransform.localScale = new Vector3(1, 1, 1);
        //Load Unit Name
        string display_name = "";
        bool all_upper_case = true;
        for (int i = 0; i < name.Length; i++)
        {
            if (name[i] >= 'A' && name[i] <= 'Z' && i != 0)
            {
                display_name += ' ';
                display_name += name[i];
            }
            else
                display_name += name[i];
            if (name[i] >= 'a' && name[i] <= 'z')
                all_upper_case = false;
        }
        if (all_upper_case)
            display_name = name;
        UIBar.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(display_name);
        BarInfo info = UIBar.GetComponent<BarInfo>();
        info.Set();

        Building b = getConstruction(name);
        string[] production = b.getProduction();
        GameObject button;
        for(int i = 0; i < production.Length; i++)
        {
            button = Instantiate(Resources.Load(production[i] + "ProductionButton") as GameObject, UIBar.transform.GetChild(info.getIndex("Production") + i));
            Instantiate(Resources.Load("ProductionElement") as GameObject, button.transform);
        }
    }

    public void SetUpConstructingBar(string name, int index)
    {
        current = currentUnits[index];
        if (UIBar != null)
            Destroy(UIBar);

        List<GameObject> currentSelected = unitselection.getCurrentSelected();
        GameObject gameobj = Resources.Load("ConstructingBar") as GameObject;
        UIBar = Instantiate(gameobj, FunctionBar.transform);
        //Load Unit Image
        GameObject picture = new GameObject("Picture");
        picture.AddComponent<RawImage>();
        Texture texture = Resources.Load(name + "Image") as Texture;
        picture.GetComponent<RawImage>().texture = texture;
        RectTransform pictureTransform = picture.GetComponent<RectTransform>();
        pictureTransform.sizeDelta = UIBar.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta;
        pictureTransform.position = UIBar.transform.GetChild(0).GetComponent<RectTransform>().position;
        picture.transform.SetParent(UIBar.transform.GetChild(0));
        pictureTransform.localScale = new Vector3(1, 1, 1);
        //Load Unit Name
        string display_name = "";
        bool all_upper_case = true;
        for (int i = 0; i < name.Length; i++)
        {
            if (name[i] >= 'A' && name[i] <= 'Z' && i != 0)
            {
                display_name += ' ';
                display_name += name[i];
            }
            else
                display_name += name[i];
            if (name[i] >= 'a' && name[i] <= 'z')
                all_upper_case = false;
        }
        if (all_upper_case)
            display_name = name;
        UIBar.transform.GetChild(1).GetComponent<TextMeshProUGUI>().SetText(display_name);
        BarInfo info = UIBar.GetComponent<BarInfo>();
        info.Set();

        float ct = current.GetComponent<Construction>().getStartTime();
        UIBar.transform.GetChild(info.getIndex("Progress")).GetComponent<Slider>().value = (Time.time - ct) / current.transform.GetChild(0).GetComponent<UnitLoad>().OutputUnit().getProduceTime();
    }

    public void RemoveTroop(int index)
    {
        if (current == null || UIBar == null || !UIBar.name.Equals("DetailBar(Clone)"))
            return;
        UnitLoad load = current.transform.GetChild(0).GetComponent<UnitLoad>();
        if (index == -1)
            load.removeAllTroop();
        else
        {
            if (index >= load.OutputUnit().getTroop())
                return;
            load.removeTroop(index);
        }
        UIBar.transform.Find("SelectionBar").gameObject.SetActive(false);
    }

    public List<List<Equipment>> getEquipment(string name)
    {
        return AllEquipments[dictionary[name]];
    }

    public void resetCurrentUnitEquipment()
    {
        if (currentUnits.Count == 0)
            return;
        List<GameObject> toRemove = new List<GameObject>();
        foreach(GameObject gameobj in currentUnits)
        {
            try
            {
                gameobj.transform.GetChild(0).GetComponent<UnitLoad>().resetEquipment();
            }
            catch(MissingReferenceException)
            {
                toRemove.Add(gameobj);
                continue;
            }
        }
        for (int i = 0; i < toRemove.Count; i++)
            currentUnits.Remove(toRemove[i]);
    }

    void detectNewUnit()
    {
        GameObject[] Array = (GameObject[])FindObjectsOfType(typeof(GameObject));
        UnitLoad com;
        for (int i = 0; i < Array.Length; i++)
        {
            if (Array[i].layer == 3 && Array[i].TryGetComponent<UnitLoad>(out com) && !contain(Array[i].GetComponent<UnitLoad>().OutputUnit()))
            {
                Array[i].GetComponent<UnitLoad>().GenerateID(Units);
                Units.Add(Array[i].GetComponent<UnitLoad>().OutputUnit());
            }

        }
        SaveSystem.SaveUnit(Units);
    }

    public GameObject getCurrent()
    {
        return current;
    }

    bool contain(Unit unit)
    {
        foreach (Unit u in Units)
        {
            if (u.getID() == unit.getID())
                return true;
        }
        return false;
    }

    int generatePlayerID()
    {
        int ID = Random.Range(0, 100);
        for (int index = 0; index < players.Count; index++)
            if (ID == players[index].getPlayerID())
                ID = generatePlayerID();
        return ID;
    }

    public Player PopCurrentPlayer()
    {
        if (players.Count == 0)
            return null;
        Player player = players[0];
        players.RemoveAt(0);
        return player;
    }

    public void Produce(string name)
    {
        ConstructionFunction cf;
        if (current.TryGetComponent<ConstructionFunction>(out cf) && !cf.Contain(name))
            return;
        else if (cf == null)
            return;

        cf.produce(name);
    }
}
