using System.Collections;
using System.Collections.Generic;
using System;

public enum Language
{
    English,
    Polish
}

public static class StaticLanguageData
{
    public static Dictionary<string, string> Load(Language language)
    {
        if (language == Language.Polish)
        {
            return new Dictionary<string, string>()
            {
                // Menu
                {"s_new_game_button", "Nowa gra" },
                {"s_credits_button", "O grze" },
                {"s_quit_button", "Wyjście" },
                {"s_quit_prompt", "Czy na pewno chcesz opuścić grę?                       " },
                {"s_retreat_prompt", "Czy na pewno chcesz przerwać misję?                 " },
          
                // O grze
                {"s_credits_programming", "Projekt i programowanie" },
                {"s_credits_icons", "Ikony" },
                {"s_credits_music", "Muzyka" },
                {"s_credits_sound", "Efekty dźwiękowe" },
                {"s_credits_fonts", "Fonty" },
                {"s_credits_gfx", "Grafika" },
                {"s_credits_scripts", "Dodatkowe skrypty" },
                {"s_credits_palette", "Paleta" },
                {"s_credits_translation", "Tłumaczenie na angielski" },
                        
                // Wiadomości
                {"s_start_text", "Na galaktycznych rynkach nie ma cenniejszego surowca niż kosmiczne kryształy. Twoim zadaniem jest zrabować je z pustynnej planety i uciec, zanim Twoja obecność zostanie wykryta przez jej właścicieli." },
                {"s_victory_text", "Udało się na czas zebrać kryształy! Twoje bogactwo będzie odtąd niezmierzone.  " },
                {"s_time_defeat_text", "Czas upłynął, trzeba uciekać! Może następnym razem się uda...              " },
                {"s_death_defeat_text", "Cała załoga zginęła! Na szczęście znajdzie się wielu kolejnych ochotników..." },
                {"s_start_confirmation", "Do roboty!" },
                {"s_victory_confirmation", "Zwycięstwo!" },
                {"s_defeat_confirmation", "A niech to." },

                // Zasoby i opisy
                {"s_crystals", "Kryształy" },
                {"s_crystals_desc", "Kryształy. Zbierane automatycznie po wybudowaniu magazynu na kryształy" },
                {"s_crystals_required", "Miejsce na kryształy" },

                {"s_metal", "Metal" },
                {"s_metal_desc", "Metal. Potrzebny do konstrukcji budynków i części zamiennych" },
                {"s_metal_required", "Miejsce na metal" },

                {"s_gas", "Gaz" },
                {"s_gas_desc", "Gaz. Wydobywany w rafinerii" },
                {"s_gas_required", "Miejsce na gaz" },

                {"s_ore", "Ruda" },
                {"s_ore_desc", "Ruda. Po wybudowaniu huty automatycznie zbierana przez kosmonautów i przetapiana na metal" },
                {"s_ore_required", "Miejsce na rudę" },

                {"s_plants", "Rośliny" },
                {"s_plants_desc", "Rośliny. Potrzebne do produkcji leków i żywności" },
                {"s_plants_required", "Miejsce na rośliny" },

                {"s_medicine", "Leki" },
                {"s_medicine_desc", "Leki. Produkowane w laboratorium, potrzebne do działania komory leczniczej" },
                {"s_medicine_required", "Miejsce na leki" },

                {"s_food", "Żywność" },
                {"s_food_desc", "Żywność. Kosmonauci spożywają ją w kwaterach" },
                {"s_food_required", "Miejsce na żywność" },

                {"s_parts", "Części zamienne" },
                {"s_parts_desc", "Części zamienne. Wykorzystywane do produkcji i naprawy robotów" },
                {"s_parts_required", "Miejsce na części zamienne" },

                {"s_robot", "Robot" },
                {"s_robot_desc", "Roboty" },
                {"s_robot_required", "Miejsce na robota" }, 

                // Potrzeby
                {"Hunger", "Głód" },
                {"HungerDesc", "Głód. Kosmonauci mogą go zaspokoić spożywając posiłek w kwaterach" },
                {"Health", "Zdrowie" },
                {"HealthDesc", "Zdrowie. Przywracane w komorze leczniczej" },
                {"Condition", "Stan" },
                {"ConditionDesc", "Stan. Odnawiany w stacji naprawczej za pomocą części zamiennych" },          

                // Budynki
                {"Spaceship", "Statek kosmiczny" },
                {"OreDeposit", "Ruda" },
                {"CrystalsDeposit", "Kryształy" },
                {"Stairs", "Schody" },
                {"Platform", "Platforma" },
                {"Slab", "Chodnik" },
                {"Greenhouse", "Szklarnia" },
                {"Storage", "Magazyn" },
                {"HealingChamber", "Komora lecznicza" },
                {"Quarters", "Kwatery" },
                {"Metalworks", "Huta" },
                {"Refinery", "Rafineria" },
                {"Laboratory", "Laboratorium" },
                {"FoodSynthesizer", "Syntezator żywności" },
                {"RobotFactory", "Fabryka robotów" },
                {"PartsFactory", "Fabryka części zamiennych" },
                {"RepairStation", "Stacja napraw" },
                {"CrystalStorage", "Magazyn na kryształy" },            

                // Opisy budynków
                {"SpaceshipDesc", "" },
                {"OreDepositDesc", "" },
                {"CrystalsDepositDesc", "" },
                {"StairsDesc", "Pozwalają na wchodzenie na platformy i skały" },
                {"PlatformDesc", "Zapewnia więcej przestrzeni na stawianie budynków" },
                {"SlabDesc", "Pozwala na szybsze poruszanie się" },
                {"GreenhouseDesc", "Tutaj hodowane są rośliny potrzebne do produkcji żywności oraz leków" },
                {"StorageDesc", "Pozwala przechowywać dowolne zasoby poza kryształami" },
                {"HealingChamberDesc", "Pozwala na poprawienie stanu zdrowia kosmonautów" },
                {"QuartersDesc", "Tutaj kosmonauci mogą spożyć posiłek i odpocząć" },
                {"MetalworksDesc", "Przetapia rudę na metal" },
                {"RefineryDesc", "W tym budynku wydobywany jest gaz. Może być postawiony tylko na piasku" },
                {"LaboratoryDesc", "Służy do wytwarzania leków dla kosmonautów" },
                {"FoodSynthesizerDesc", "Syntezator produkuje żywność z roślin" },
                {"RobotFactoryDesc", "Produkuje roboty pomagające kosmonautom w pracy" },
                {"PartsFactoryDesc", "Produkuje części potrzebne do konstrukcji i naprawy robotów" },
                {"RepairStationDesc", "Służy do naprawy uszkodzonych robotów" },
                {"CrystalStorageDesc", "Tylko w tym magazynie można przechowywać kryształy" },

                // Budowa
                {"s_halted", "Wstrzymane" },
                {"s_scaffolding_construction", "Budowa rusztowania" },
                {"s_scaffolding_deconstruction", "Rozbiórka rusztowania" },
                {"s_construction_site", "Plac budowy" },
                {"s_deconstruction_site", "Rozbiórka" },

                {"s_cancel_deconstruction_button", "Przywróć budynek" },
                {"s_deconstruction_button", "Rozbiórka" },
                {"s_halt_button", "Wstrzymaj" },
                {"s_start_button", "Wznów" },
                {"s_deconstruction_prompt", "Czy na pewno chcesz wyburzyć ten budynek?           " },

                // Wskazówki
                {"s_tips_1", "Sterowanie: \n\n - WSAD / strzałki - ruch kamery \n - Q, E - obrót kamery \n - Spacja - aktywna pauza \n - R - obrót budynku                                     " },
                {"s_tips_2", "Postaci same decydują o tym, gdzie się udać i co zrobić.\n\nŻeby upewnić się, że koncentrują się na właściwych zadaniach, warto nie zlecać zbyt wielu nowych konstrukcji naraz oraz korzystać z możliwości tymczasowego wstrzymywania pracy budynków.                                    " },
                {"s_tips_3", "Postaci przystępują do zbierania kryształów automatycznie po wybudowaniu magazynów na kryształy.\n\nW pierwszej kolejności lepiej jednak zadbać o potrzeby załogi oraz wyprodukowanie dużej liczby robotów do pomocy - bez nich trudno będzie zdążyć na czas.\n\nKliknięcie na postać powoduje wyświetlenie poziomu zaspokojenia jej potrzeb. Jeśli spadnie do zera, postać umrze.                                    " },
                {"s_tips_4", "Dobrym pomysłem jest częste korzystanie z aktywnej pauzy, pozwalającej nie tracić cennych sekund podczas zastanawiania się.              " },
                {"s_tips_next", "Dalej" },
                {"s_tips_end", "OK" },

                // Inne
                {"s_stats_button", "Statystyki" },
                {"s_build_button", "Budowanie" },
                {"s_sound_on_button", "Przywróć dźwięk" },
                {"s_sound_off_button", "Wycisz" },
                {"s_tips_button", "Wskazówki" },
                {"s_timer_hover", "Czas pozostały do zakończenia misji" },
                {"s_counter_hover", "Kryształy do zebrania" },
                {"s_humans_desc", "Załoga" },
                {"s_yes", "Tak" },
                {"s_no", "Nie" },
            };
        }
        else if (language == Language.English)
        {
            return new Dictionary<string, string>()
            {                                
                // Menu
                {"s_new_game_button", "New Game" },
                {"s_credits_button", "About" },
                {"s_quit_button", "Quit" },
                {"s_quit_prompt", "Are you sure you want to quit?                               " },
                {"s_retreat_prompt", "Are you sure you want to abandon the mission?             " },
          
                // O grze
                {"s_credits_programming", "Design and Programming" },
                {"s_credits_icons", "Icon Design" },
                {"s_credits_music", "Music" },
                {"s_credits_sound", "Sound Effects" },
                {"s_credits_fonts", "Fonts" },
                {"s_credits_gfx", "Graphics" },
                {"s_credits_scripts", "Additional Scripts" },
                {"s_credits_palette", "Color Palette" },
                {"s_credits_translation", "Translation" },
                        
                // Wiadomości
                {"s_start_text", "Space crystals are the most valuable resource on the intergalactic market. Your mission is to steal them from a desert planet and escape before your presence is detected by its owners." },
                {"s_victory_text", "You successfully gathered the crystals! Your wealth shall now be unsurpassed.  " },
                {"s_time_defeat_text", "Time's out, you need to run! Maybe next time...               " },
                {"s_death_defeat_text", "The entire crew is dead! Fortunately there are always more volunteers...         " },
                {"s_start_confirmation", "Let's get to work!" },
                {"s_victory_confirmation", "Victory!" },
                {"s_defeat_confirmation", "Too bad." },

                // Zasoby i opisy
                {"s_crystals", "Crystals" },
                {"s_crystals_desc", "Crystals. Gathered automatically after you construct the Crystal Storage facility" },
                {"s_crystals_required", "Crystal Slot" },

                {"s_metal", "Steel" },
                {"s_metal_desc", "Steel. Required to construct facilities and spare parts" },
                {"s_metal_required", "Steel Slot" },

                {"s_gas", "Gas" },
                {"s_gas_desc", "Gas. Extracted in the Refinery" },
                {"s_gas_required", "Gas Slot" },

                {"s_ore", "Ore" },
                {"s_ore_desc", "Ore. Gathered automatically by your crew after constructing the Foundry and then smeltered into steel" },
                {"s_ore_required", "Ore Slot" },

                {"s_plants", "Plants" },
                {"s_plants_desc", "Plants. Required to produce medicine and food" },
                {"s_plants_required", "Plant Slot" },

                {"s_medicine", "Medicine" },
                {"s_medicine_desc", "Medicine. Produced in the Laboratory, required for the functioning of the Healing Chamber" },
                {"s_medicine_required", "Medicine Slot" },

                {"s_food", "Food" },
                {"s_food_desc", "Food. Consumed by your astronauts in Living Quarters" },
                {"s_food_required", "Food Slot" },

                {"s_parts", "Spare Parts" },
                {"s_parts_desc", "Spare Parts. Used to build and repair robots" },
                {"s_parts_required", "Spare Parts Slot" },

                {"s_robot", "Robot" },
                {"s_robot_desc", "Robots" },
                {"s_robot_required", "Robot Slot" }, 

                // Potrzeby
                {"Hunger", "Hunger" },
                {"HungerDesc", "Hunger. Your astronauts can sate it by eating a meal in their Living Quarters" },
                {"Health", "Health" },
                {"HealthDesc", "Health. Restored in the Healing Chamber" },
                {"Condition", "Condition" },
                {"ConditionDesc", "Condition. Restored in the Repair Station using spare parts" },          

                // Budynki
                {"Spaceship", "Spaceship" },
                {"OreDeposit", "Ore" },
                {"CrystalsDeposit", "Crystals" },
                {"Stairs", "Stairs" },
                {"Platform", "Platform" },
                {"Slab", "Pavement" },
                {"Greenhouse", "Greenhouse" },
                {"Storage", "Storage" },
                {"HealingChamber", "Healing Chamber" },
                {"Quarters", "Living Quarters" },
                {"Metalworks", "Foundry" },
                {"Refinery", "Refinery" },
                {"Laboratory", "Laboratory" },
                {"FoodSynthesizer", "Food Synthesizer" },
                {"RobotFactory", "Robot Factory" },
                {"PartsFactory", "Spare Parts Factory" },
                {"RepairStation", "Repair Station" },
                {"CrystalStorage", "Crystal Storage" },            

                // Opisy budynków
                {"SpaceshipDesc", "" },
                {"OreDepositDesc", "" },
                {"CrystalsDepositDesc", "" },
                {"StairsDesc", "Allows you to reach platforms and rock formations" },
                {"PlatformDesc", "Gives more space for new buildings" },
                {"SlabDesc", "Allows for faster movement" },
                {"GreenhouseDesc", "This is where you grow plants required for the production of food and medicine" },
                {"StorageDesc", "Allows you to store any resource except for crystals" },
                {"HealingChamberDesc", "Allows you to improve the health of your astronauts" },
                {"QuartersDesc", "Here your astronauts can have a meal and some rest" },
                {"MetalworksDesc", "Smelters ore into steel" },
                {"RefineryDesc", "This building allows for the extraction of gas. It can only be constructed on sand" },
                {"LaboratoryDesc", "Used to make medicine for your astronauts" },
                {"FoodSynthesizerDesc", "Synthesizes food out of plants" },
                {"RobotFactoryDesc", "Builds robots that assist your astronauts in their work" },
                {"PartsFactoryDesc", "Produces parts required to build and repair robots" },
                {"RepairStationDesc", "Used to repair damaged robots" },
                {"CrystalStorageDesc", "Special storage facility for crystals only" },

                // Budowa
                {"s_halted", "On Hold" },
                {"s_scaffolding_construction", "Scaffolding Construction" },
                {"s_scaffolding_deconstruction", "Scaffolding Demolition" },
                {"s_construction_site", "Construction Site" },
                {"s_deconstruction_site", "Demolition Site" },

                {"s_cancel_deconstruction_button", "Cancel Demolition" },
                {"s_deconstruction_button", "Demolish" },
                {"s_halt_button", "Hold" },
                {"s_start_button", "Continue" },
                {"s_deconstruction_prompt", "Are you sure you want to demolish this building?              " },

                // Wskazówki
                {"s_tips_1", "Controls: \n\n - WSAD / arrow keys - camera movement \n - Q, E - camera rotation \n - Space - active pause \n - R - rotate building                                     " },
                {"s_tips_2", "Your crew automatically decides what to do and where to go. \n\nTo make sure it is focused on the right tasks, it is recommended to limit the number of concurrent construction sites and to use the option to put a construction on hold.                                    " },
                {"s_tips_3", "Your crew automatically begins to harvest crystals after you construct the Crystal Storage facility.\n\nIt is best to first ensure that your crew's needs are taken care of and to build a lot of robots to assist them - without them it may be difficult to make it on time.\n\nClicking on a crew member will show their hunger and health levels. If either drops to zero, the crew member dies." },
                {"s_tips_4", "It is always a good idea to use the active pause to save valuable seconds while considering your next move.              " },
                {"s_tips_next", "Next" },
                {"s_tips_end", "OK" },

                // Inne
                {"s_stats_button", "Stats" },
                {"s_build_button", "Build" },
                {"s_sound_on_button", "Sound On" },
                {"s_sound_off_button", "Sound Off" },
                {"s_tips_button", "Hints" },
                {"s_timer_hover", "Time Remaining" },
                {"s_counter_hover", "Crystals to Harvest" },
                {"s_humans_desc", "Crew" },
                {"s_yes", "Yes" },
                {"s_no", "No" },
            };
        }
        else
        {
            return null;
        }
    }
}

