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
                {"s_quit_prompt", "Czy na pewno chcesz opuścić grę?" },
                {"s_retreat_prompt", "Czy na pewno chcesz przerwać misję?" },
          
                // O grze
                {"s_credits_programming", "Projekt i programowanie" },
                {"s_credits_icons", "Ikony" },
                {"s_credits_music", "Muzyka" },
                {"s_credits_sound", "Efekty dźwiękowe" },
                {"s_credits_fonts", "Fonty" },
                {"s_credits_gfx", "Grafika" },
                {"s_credits_scripts", "Dodatkowe skrypty" },
                {"s_credits_palette", "Paleta" },
                        
                // Wiadomości
                {"s_start_text", "Na galaktycznych rynkach nie ma cenniejszego surowca niż kosmiczne kryształy. Twoim zadaniem jest zrabować je z pustynnej planety i uciec, zanim Twoja obecność zostanie wykryta przez jej właścicieli." },
                {"s_victory_text", "Udało się na czas zebrać kryształy! Twoje bogactwo będzie odtąd niezmierzone.  " },
                {"s_time_defeat_text", "Czas upłynął, trzeba uciekać! Może następnym razem się uda..." },
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
                {"s_deconstruction_prompt", "Czy na pewno chcesz wyburzyć ten budynek?" },

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
                {"s_new_game_button", "New game" },
                {"s_credits_button", "About" },
                {"s_quit_button", "Exit" },

                {"s_start_text", "Wersja angielska" },
            };
        }
        else
        {
            return null;
        }
    }
}

