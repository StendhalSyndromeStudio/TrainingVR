using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts.Menus.Lecturer.MenuTraining.PrefabController {
    public class TargerController : UnityEngine.MonoBehaviour {

        void Awake( ) {
            PrefabController.Controller.Current = this.gameObject;
        }
    }
}
