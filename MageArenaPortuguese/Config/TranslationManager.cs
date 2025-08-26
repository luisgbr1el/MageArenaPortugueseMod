using System.Collections.Generic;

namespace MageArenaPortuguese.Config
{
    public static class TranslationManager
    {
        private static readonly Dictionary<string, Dictionary<string, string>> translations
            = new Dictionary<string, Dictionary<string, string>>();

        private static bool isLoaded = false;

        public enum InteractionType
        {
            Pegar,
            Colocar,
            Fazer,
            Abrir,
            Fechar
        }

        // Dicas mostradas no jogo
        private static readonly Dictionary<string, string> tips = new Dictionary<string, string>
        {
            { "Scroll to equip your torch", "Gire a roda do mouse para equipar sua tocha" },
            { "Hold LMB to light the brazier", "Segure o botão esquerdo para acender o braseiro" },
            { "[Press RMB to Continue]", "[Pressione o botão direito do mouse para continuar]" },
            { "There are flag poles in each team's base and throughout the map. Hold Q to view the map. [Press RMB to Continue]", "Há bandeiras em cada base do time e espalhadas pelo mapa. Segure Q para ver o mapa. [Pressione o botão direito do mouse para continuar]" },
            { "If the flag pole in your base is captured, you will not be resurrected when you die. Recall to your base by pressing B. [Press RMB to Continue]", "Se a bandeira da sua base for capturada, você não será ressuscitado quando morrer. [Pressione o botão direito do mouse para continuar]" },
            { "The orb in the bottom of your screen indicates if you will resurrect upon death. A red skull in the orb means you will die forever. [Press RMB to Continue]", "O orbe na parte inferior da tela indica se você vai reviver ao morrer. Uma caveira vermelha no orbe significa que você morrerá para sempre. [Pressione o botão direito do mouse para continuar]" },
            { "To achieve Victory, control the flag pole in your opponents base and eliminate each of your opponents. [Press RMB to Continue]", "Para conseguir a vitória, domine a bandeira da base dos seus oponentes e elimine-os. [Pressione o botão direito do mouse para continuar]" },
            { "Controlling flagpoles around the map grants you resources and XP. [Press RMB to Continue]", "Dominar as bandeiras pelo mapa dá a você recursos e XP. [Pressione o botão direito do mouse para continuar]" },
            { "Stand Next to the flag pole to capture it.", "Fique ao lado da bandeira para capturá-la." },
            { "Press E to pickup the Frog and Log and place them on the crafting table to craft a Frog Spear.", "Pressione E para pegar o Sapo e o Tronco, e coloque-os na mesa de criação para criar uma Lança de Sapo." },
            { "Equip your spellbook, press 1 to turn to the page Fireball. Then say the word \"Fireball\" to cast Fireball", "Equipe seu Livro de Feitiços e pressione 1 para abrir a página da Bola de Fogo. Então, diga \"fogo\" ou \"chamas\" para lançá-la." },
            { "Press 2 to turn to the page Frost Bolt. Then say the word \"Freeze\" to cast Frost Bolt", "Pressione 2 para abrir a página do Raio de Gelo. Então diga \"gelo\" ou \"congelar\" para lançá-lo." },
            { "Press 3 to turn to the page Worm Hole. Then say the word \"Worm\" to cast the first half of Worm Hole", "Pressione 3 para abrir a página do Buraco de Minhoca. Então diga \"entrada\" para lançar a primeira parte do Buraco de Minhoca." },
            { "Now move to a new location and say the word \"Hole\" to cast the second half of Worm Hole", "Agora vá até um novo local e diga \"saída\" para lançar a segunda parte do Buraco de Minhoca." },
            { "Press 4 to turn to the page Magic Missle. Then say the words \"Magic Missle\" to cast Magic Missle", "Pressione 4 para abrir a página do Míssil Mágico. Então diga \"míssil mágico\" para lançá-lo." },
            { "You have passed the Academy of Sorceries final examination. You may now exit the tutorial.", "Você passou no exame final da Academia de Feitiçaria. Agora pode sair do tutorial." },
            { "A strong magical presence dispells your worm...", "Uma forte presença mágica dissipa a entrada do seu Buraco de Minhoca..." },
            { "A strong magical presence dispells your hole...", "Uma forte presença mágica dissipa a saída do seu Buraco de Minhoca..." },
            { "A surge of magical energy passes through you...", "Uma onda de energia mágica passa por você..." },
            { "Your spells grow stronger...", "Seus feitiços ficam mais fortes..." },
            { "Your voice bellows from the mountain tops...", "Sua voz ecoa dos topos das montanhas..." },
            { "Your joints feel like rubber...", "Suas articulações parecem borracha..." },
            { "Your tounge gains a mind of its own...", "Sua língua ganha vida própria..." },
            { "Your skin hardens like tree bark...", "Sua pele endurece como casca de árvore..." }
        };

        // Textos dos menus
        private static readonly Dictionary<string, string> menu = new Dictionary<string, string>
        {
            { "Play Tutorial", "Jogar Tutorial" },
            { "Create Lobby", "Criar Lobby" },
            { "Join Lobby", "Entrar no Lobby" },
            { "Credits", "Créditos" },
            { "Quit Game", "Sair do Jogo" },
            { "ATTENTION!", "ATENÇÃO!" },
            { "People with non-American accents please click here", "Sotaque não-americano?\nClique aqui." },
            { "Accent Menu", "Menu de Sotaques" },
            { "Voice Detection tips:\n\nSay spell names once, do not say them over and over.\n\nYour microphone peaking too long will cause voice recognition to fail.\n\nPlace your mic further away from you if you want to scream spells, and do not scream directly into it.\n\nThe game has to use your windows default microphone, this can be changed in windows sound settings", "Tradução em Português por @luisgbr1el\n\nDicas de detecção de voz:\n\nDiga o nome dos feitiços apenas uma vez, não os repita várias vezes.\n\nSe o seu microfone ficar saturado por muito tempo, o reconhecimento de voz pode falhar.\n\nColoque o microfone mais distante de você se quiser gritar os feitiços e não grite diretamente nele.\n\nO jogo precisa usar o microfone padrão do Windows, isso pode ser alterado nas configurações de som do Windows." },
            { "Edit Team Flag", "Editar bandeira" },
            { "Kick", "Remover" },
            { "Are you sure?", "Tem certeza?" },
            { "Generating World...", "Gerando mundo..." },
            { "Reset", "Resetar" },
            { "Player 1", "Jogador 1" },
            { "Back", "Voltar" },
            { "Capture the flag", "Capture a bandeira" },
            { "Capture the Flag", "Capture a bandeira" },
            { "British", "Britânico" },
            { "Traits", "Características" },
            { "Reset Voice Detection", "Resetar detecção de voz" },
            { "Fullscreen", "Tela cheia" },
            { "The Host Leaving will disband the lobby", "Se o host sair, o lobby será desfeito" },
            { "Play Again", "Jogar novamente" },
            { "American", "Americano" },
            { "Deathmatch", "Mata-mata" },
            { "MB3", "Rol." },
            { "Start Game", "Iniciar jogo" },
            { "!Public lobbies are expierencing a large number of game breaking bugs!", "Os lobbies públicos estão com um grande número de bugs críticos!" },
            { "LMB", "Botão esq." },
            { "Send", "Enviar" },
            { "Players will not be able to join past this point.", "Os jogadores não poderão entrar a partir deste ponto." },
            { "Kick Player Menu", "Menu de expulsão de jogador" },
            { "Make Private", "Privado" },
            { "Make Public", "Público" },
            { "Error version mismatch! Restart Steam to update.", "Erro de incompatibilidade de versão! Reinicie o Steam para atualizar." },
            { "Leave Game", "Sair do jogo" },
            { "(Click if you cant cast spells)", "(Se não conseguir lançar feitiços)" },
            { "Indian", "Indiano" },
            { "Settings", "Configurações" },
            { "Save to Disk", "Salvar no disco" },
            { "Colosseum", "Coliseu" },
            { "Spacebar", "Barra de espaço" },
            { "Large", "Grande" },
            { "Leave Lobby", "Sair do Lobby" },
            { "Join Sorcerers", "Juntar-se aos Feitiçeiros" },
            { "Join Warlocks", "Juntar-se aos Bruxos" },
            { "Copy LobbyID to\nClipboard", "Copiar ID" },
            { "Start Lobby", "Iniciar Lobby" },
            { "Soups Drank", "Sopas tomadas" },
            { "Kills", "Abates" },
            { "Medium", "Médio" },
            { "Windowed", "Modo janela" },
            { "Borderless", "Sem bordas" },
            { "Find a public match", "Procurar uma partida pública" },
            { "Enter Lobby", "Entrar no Lobby" },
            { "Leave", "Sair" },
            { "Distance Traveled", "Distância percorrida" },
            { "Join private by Lobby ID:", "Entrar por ID:" },
            { "Version 0.7.6", "Versão 0.7.6" },
            { "Deaths", "Mortes" },
            { "Spells Cast", "Feitiços lançados" },
            { "Load from disk", "Carregar do disco" },
            { "Refresh", "Atualizar" },
            { "Filter by", "Filtrar por" },
            { "Your spells gain power...", "Seus feitiços ganham poder..." },
            { "Click to Reveal Lobby ID", "Clique para ver o ID do Lobby" },
            { "Enter text...", "Digite aqui..." },
            { "The only available language currently is English.\nMultiple Languages will be available in a later update.\n\nUnfortunately, accents can cause the voice detection to not work as well.\n\nIf you do not have one of the following accents, try out all three.\n\n\n\n\n\n\nThese three are the only English accent options available.", "O único idioma disponível nativamente atualmente é o inglês.\n(Tradução em Português por luisgbr1el)\nMais idiomas estarão disponíveis em uma atualização futura.\n\nInfelizmente, sotaques podem fazer com que a detecção de voz não funcione bem.\n\nSe você não tiver um dos sotaques abaixo, experimente os três.\n\n\n\n\n\n\nEssas são as únicas opções de sotaque em inglês disponíveis." },
            { "This will overwrite\nthe current drawing!", "Isso irá sobrescrever\no desenho atual!" },
            { "Drop", "Soltar" },
            { "Flip Page", "Virar página" },
            { "Licensed textures obtained from freepbr.com", "Texturas licenciadas obtidas de freepbr.com" },
            { "Movement Keys", "Teclas de movimento" },
            { "Error Lobby ID did not generate. Sorry. Try relaunching the game.", "Erro: O ID do Lobby não foi gerado. Tente reiniciar o jogo." },
            { "Map Size:", "Tamanho do mapa:" },
            { "title", "title [teste]" },
            { "Microphone is the Windows Default Mic this can not be changed. Check in Windows sound settings.", "O microfone é o padrão do Windows. Abra as configurações de som do Windows para alterar." },
            { "Restore Defaults", "Redefinir padrões" },
            { "Lobby ID: (send this to your friends)", "ID do Lobby: (mande para seus amigos)" },
            { "Push to talk has been temporarily disabled", "O aperte para falar está desativado" },
            { "Mouse Sensitivity", "Sensibilidade do mouse" },
            { "Jump", "Pular" },
            { "Interact", "Interagir" },
            { "Crouch", "Agachar" },
            { "Sprint", "Correr" },
            { "Save", "Salvar" },
            { "Map", "Mapa" },
            { "Contact me on twitter/x: @jrsjams_", "Contate-me no Twitter/X: @jrsjams_\nTradução em Português por @luisgbr1el" },
            { "Many Sound effects obtained from ZapSplat at zapsplat.com", "Muitos efeitos sonoros obtidos de ZapSplat em zapsplat.com" },
            { "This will overwrite\nyour previously saved flag!", "Isso irá sobrescrever sua\n bandeira salva anteriormente!" },
            { "Limit Fps", "Limitar FPS" },
            { "Use Item", "Usar item" },
            { "Screen Mode:", "Modo de tela:" },
            { "Quick Swap Item Keys", "Teclas de troca rápida" },
            { "British has been disabled,\nIt did not work for anyone", "O britânico foi desativado,\nnão está funcionando." },
            { "Deathmatch 1v1", "Mata-mata 1v1" },
            { "Recall", "Voltar\np/ base" },
            { "Option A", "Opção A" },
            { "Select Brush Size:", "Tamanho do pincel:" }
        };

        // Interagíveis do jogo (itens, mesas de criação, etc)
        private static readonly Dictionary<string, string> interactable = new Dictionary<string, string>
        {
            // Interações
            { "Move to Close", "Mova-se para fechar" },
            { "Read Crafting Manual", "Ler manual de criação" },
            { "Speak to Soup Man", "Falar com o Homem-Sopa" },
            { "SpeaK to Knight", "Falar com o Cavaleiro" },
            { "Enter Mausoleum", "Entrar no Mausoléu" },
            { "Exit Mausoleum", "Sair do Mausoléu" },
            { "Mirror mirror...", "Espelho, espelho meu..." },
            { "Trade Item", "Trocar Item" },
            { "Item", "Item" },
            { "Dore", "Porta" },

            // Interagíveis
            { "Chest", "Baú" },
            { "the Portcullis", "a grade" },
            { "Torn Page", "Folha Rasgada" },

            // Itens
            { "Bounce Mush", "Cogumelo Saltitante" },
            { "Crystal", "Cristal" },
            { "Pull Hilt of Excalibur", "Puxar Cabo da Excalibur" },
            { "Log", "Tronco" },
            { "RocK", "Pedra" },
            { "Pull Shattered Blade of Excalibur", "Puxar Lâmina Estilhaçada da Excalibur" },
            { "Toad", "Sapo" },

            // Sopas
            { "Crystal Soup", "Sopa de Cristal" },
            { "Frog Soup", "Sopa de Sapo" },
            { "Log Soup", "Sopa de Tronco" },
            { "Mushroom Soup", "Sopa de Cogumelo" },
            { "Rock Soup", "Sopa de Pedra" },

            // Craftáveis
            { "Boinger", "Saltador" },
            { "Dart Gun", "Sapo-Dardo" },
            { "Excalibur", "Excalibur" },
            { "Frog Balloon", "Balão de Sapo" },
            { "Frog Spear", "Vareta de Sapo" },
            { "Frogleather Blade", "Espada de Couro de Sapo" },
            { "Fungal Walking Stick", "Bengala Fúngica" },
            { "Golum", "Golem" },
            { "Levitator", "Levitador" },
            { "Mushroom Man", "Homem-Cogumelo" },
            { "Mushroom Swerd", "Espada-Cogumelo" },
            { "Ray of Shrink", "Raio Encolhedor" },
            { "Silverseed Bramble", "Semente-de-Prata Espinhosa" },
            { "Spore Frog", "Sapo de Esporos" },
            { "The Orb", "Orbe" },
            { "Thrumming Stone", "Pedra Vibrante" },
            { "Walking Stick", "Bengala" },

            // Ferramentas
            { "BooK", "Livro de Feitiços" },
            { "Torch", "Tocha" },
            { "Weed of the Pipe", "Erva do Cachimbo" }
        };

        private static void LoadTranslations()
        {
            translations["tips"] = tips;
            translations["menu"] = menu;
            translations["interactable"] = interactable;

            isLoaded = true;
        }

        public static string Get(string category, string key, InteractionType? interactionType = null)
        {
            if (!isLoaded)
                LoadTranslations();

            string normalizedKey = key.Trim();
            string translation;

            if (translations.ContainsKey(category) && translations[category].ContainsKey(normalizedKey))
                translation = translations[category][normalizedKey];
            else
                return key;

            if (interactionType.HasValue)
                return $"{interactionType} {translation}";

            return translation;
        }
    }
}