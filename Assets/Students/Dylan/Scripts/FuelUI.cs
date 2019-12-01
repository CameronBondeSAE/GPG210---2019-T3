using System.Collections;
using System.Collections.Generic;
using Students.Luca.Scripts.Checkpoints;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FuelUI : MonoBehaviour
{

    private Vector3 currentSpeedAngle;
    [SerializeField]
    //private Vector3 maxSpeedAngle;

    private float maxSpeedAngle = -180;
    private float zeroSpeedAngle = 60;
    private float maxSpeed;
    private float currentSpeed;
    
    //private Quaternion speedOMeterHandRot;
    private float fuel;
    private float maxFuel;
    //private DylanCar dylanCar;
    //public Transform speedOMeterHand;
    //private Transform speedLabelTemplateTransform;
    //public Text speedLabelText;
    private Fuel _fuel;
    public TextMeshProUGUI fuelText;
    private PlayerManager _playerManager;
    private void Start()
    {
        //_playerManager = FindObjectOfType<PlayerManager>();
        //dylanCar = FindObjectOfType<DylanCar>();
        //fuel = _playerManager.playerInfos[0].controller.possessable.gameObject.GetComponent<Fuel>().currentFuel;
        //the below is getting a reference to the max fuel from the fuel component script
        //maxFuel = fuelComponent.maxFuel;
        //fuel = fuelComponent.currentFuel;
        //fuelText.text = "Fuel: " + fuel.ToString("F0");
        //peedLabelTemplateTransform = transform.Find("SpeedLabelTemplate");
        //speedLabelTemplateTransform.gameObject.SetActive(false);

        //currentSpeed = 0f;
        //maxSpeed = 200f;
        
    }

    public void FuelInit(PlayerInfo playerInfo)
    {
        _fuel = playerInfo.controller.possessable.gameObject.GetComponent<Fuel>();
        fuel = _fuel.currentFuel;
        maxFuel = _fuel.maxFuel;
    }
    private void Update()
    {
        fuel = _fuel.currentFuel;
        
        fuelText.text = "Fuel: " + fuel.ToString("F0");

        if (_fuel.OutOfFuel)
        {
            fuelText.text = "Fuel: Out Of Fuel";
        }
        if(fuel >= maxFuel)
        {
            fuel = maxFuel;
        }
        /*
        currentSpeedAngle = new Vector3(0, 0, -dylanCar.speed);
        speedOMeterHandRot.eulerAngles = currentSpeedAngle;
        speedOMeterHand.transform.rotation = speedOMeterHandRot;
        */
        
        
        if (currentSpeed > maxSpeed)
        {
            currentSpeed = maxSpeed;
        }

        //speedOMeterHand.eulerAngles = new Vector3(0, 0, GetSpeedRotation());
        //CreateSpeedLabels();
    }

    /* was a test for a speedometer, but labels won't spawn correctly and not sure why
    private void CreateSpeedLabels()
    {
        int labelAmount = 10;
        float totalAngelSize = zeroSpeedAngle - maxSpeedAngle;

        for (int i = 0; i <= labelAmount; i++)
        {
            Transform speedLabelTransform = Instantiate(speedLabelTemplateTransform, transform);
            float labelSpeedNormalised = (float) i / labelAmount;
            float speedLabelAngle = zeroSpeedAngle - labelSpeedNormalised * totalAngelSize;
            speedLabelTransform.eulerAngles = new Vector3(0,0,speedLabelAngle);
            speedLabelTransform.Find("SpeedText").GetComponent<Text>().text = Mathf.RoundToInt(labelSpeedNormalised * maxSpeed).ToString();
            speedLabelTransform.Find("SpeedText").eulerAngles = Vector3.zero;
            speedLabelTransform.gameObject.SetActive(true);

        }
    }
    private float GetSpeedRotation()
    {
        float totalAngleSize = zeroSpeedAngle - maxSpeedAngle;

        float speedNormalised = currentSpeed / maxSpeed;

        return zeroSpeedAngle - speedNormalised * totalAngleSize;
    }
    */

    
}
