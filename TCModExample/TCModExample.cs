using ANToolkit;
using ANToolkit.Controllers; 
using ANToolkit.Level;
using ANToolkit.Save;
using Asuna.CharManagement;
using Asuna.Dialogues;
using Asuna.Items;
using Modding;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TCModExample
{
    public class TCModExample : ITCMod
    {
        List<Dialogue> dialogues;


        public void OnDialogueStarted(Dialogue dialogue) { }

        public void OnFrame(float deltaTime) { }

        public void OnLevelChanged(string oldLevel, string newLevel)
        {
            if (newLevel == "Peitho Training Base")
            {
                var character = Character.Get("Dr. Weber");
                var handler = CharacterHandler.Create(character);
                handler.transform.position = new Vector3(-54.582f, -3.185f);

                var controller = handler.GetComponent<CharController>();
                controller.FacingDirection = MoveDirection.Down;

                var interactable = handler.gameObject.AddComponent<Interactable>();
                interactable.TypeOfInteraction = InteractionType.Talk;
                interactable.OnInteracted.AddListener(x =>
                {
                    bool HasRecievedItem = SaveManager.GetKey("ModItemRecieved", false);
                    if (!HasRecievedItem) {
                        DialogueManager.StartDialogue(dialogues[0]);
                        SaveManager.SetKey("ModItemRecieved", true);

                        var item = Item.Create<Apparel>("Modded Purple Collar");
                        GiveItems.GiveToCharacter(Character.Get("Jenna"), true, true, item);
                    } else
                        DialogueManager.StartDialogue(dialogues[1]);

                });
            }
        }

        public void OnLineStarted(DialogueLine line) { }

        public void OnModLoaded(ModManifest manifest)
        {
            dialogues = new List<Dialogue> {
            DialogueIngameEditor.LoadDialogue(Path.Combine(manifest.ModPath, "Dialogue\\WeberGiveItem.dialogue")),
            DialogueIngameEditor.LoadDialogue(Path.Combine(manifest.ModPath, "Dialogue\\WeberGiveItem2.dialogue"))
            };

            new ModEquipment()
            {
                Name = "Modded Purple Collar",
                Slots = new List<string>()
                    {
                        EquipmentSlot.Choker.ToString()
                    },
                PreviewImage = Path.Combine(manifest.ModPath, "Equipment\\PurpleCollarPreview.png"),
                Sprites = new List<ModEquipmentSprite>()
                    {
                        new ModEquipmentSprite()
                        {
                            Name = "Shirt",
                            Image = Path.Combine(manifest.ModPath, "Equipment\\PurpleCollar.png"),
                            Color = "#FFFFFFFF",
                            SortingLayer = ApparelLayer.Outfit.ToString(),
                            SortingOrder = 0
                        }
                    }
            }.Initialize(manifest.SpriteResolver);
        }

        public void OnModUnLoaded() { }
    }
}
