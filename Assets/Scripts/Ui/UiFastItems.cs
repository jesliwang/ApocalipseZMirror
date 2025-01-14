using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ApocalipseZ
{
    public class UiFastItems : MonoBehaviour
    {
        public UISlotItem PrefabSlot;
        private List<UISlotItem> FastSlot = new List<UISlotItem>();
        private Transform Container;

        void Awake()
        {
            Container = transform.Find("Container").transform;
        }
        public void AddSlots(FpsPlayer FpsPlayer)
        {
            Inventory inventory = FpsPlayer.GetInventory();
            WeaponManager weaponManager = FpsPlayer.GetWeaponManager();
            FastItemsManager fastItemsManager = FpsPlayer.GetFastItemsManager();
            foreach (UISlotItem item in FastSlot)
            {
                Destroy(item.gameObject);
            }
            FastSlot.Clear();
            for (int i = 0; i < fastItemsManager.container.Count; i++)
            {
                UISlotItem instance = Instantiate(PrefabSlot, Container);
                instance.HUD = transform.parent;
                instance.SetTypeContainer(TypeContainer.FASTITEMS);
                instance.SetSlotIndex(i);
                instance.SetInventory(inventory);
                instance.SetWeaponManager(weaponManager);
                instance.SetFastItemsManager(fastItemsManager);
                FastSlot.Add(instance);
            }
        }

        internal void UpdateSlot(int index, SlotInventoryTemp newItem)
        {

            DataItem dataItem = GameController.Instance.DataManager.GetDataItemById(newItem.guidid);
            if (dataItem == null)
            {
                FastSlot[index].SetIsEmpty(true);
                FastSlot[index].SetImage(null);
                FastSlot[index].SetTextQuantidade("");
            }
            else
            {
                FastSlot[index].SetIsEmpty(false);
                FastSlot[index].SetImage(dataItem.Thumbnail);
                FastSlot[index].SetTextQuantidade(newItem.Quantity.ToString());
            }


        }

    }
}