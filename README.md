
# Mage Arena Portuguese Mod

> Mod para Mage Arena que traduz todos os **textos** e permite invocar feitiços por comandos de voz em português.

---

## ✨ **Funcionalidades**

- Tradução completa dos textos do jogo para português.
- Permite usar comandos de voz em português para invocar feitiços.
- Compatível com comandos de voz originais em inglês.
- Suporte a variantes e sinônimos dos nomes dos feitiços.
- Compatível com outros mods que não alteram o sistema de voz ou tradução.
- Suporte a modelos de reconhecimento de voz em português (Recognissimo, Vosk).

---

## 🗣️ **Casts de Voz Suportados**
Listas de casts de voz suportados pelo mod, incluindo plugins.
### Feitiços nativos do jogo

| Feitiço (inglês)       | Cast em português                       | Variantes/Sinônimos                                             |
|------------------------|----------------------------------------|------------------------------------------------------------------|
| Fireball               | fogo, chamas                            | fireball, fire, ball                                            |
| Freeze                 | congelar, gelo                          | freeze                                                          |
| Wormhole (entrada)     | entrada                                 | worm                                                            |
| Wormhole (saída)       | saída                                   | hole                                                            |
| Magic Missile          | míssil mágico                           | missil magico, magic, missile                                   |
| Mirror                 | espelho                                 | mirror                                                          |
| Rock                   | rocha, pedra                            | rock                                                            |
| Wisp                   | lento, lentidão                         | wisp                                                            |
| Dark Blast             | explosão escura, explosão               | explosao escura, explosao, dark, blast                          |
| Divine Light           | luz divina, luz                         | divine                                                          |
| Blink                  | clarão, teletransporte, piscar          | clarao, blink                                                   |
| Thunderbolt            | raio, relâmpago                         | relampago, thunderbolt                                          |

### Feitiços do Plugin "MoreSpells"
| Feitiço (inglês)       | Cast em português                       | Variantes/Sinônimos                                             |
|------------------------|----------------------------------------|------------------------------------------------------------------|
| Resurrection           | ressurreição                            | ressureicao, resurrection                                       |
| Magic Shield           | escudo mágico, escudo                   | magic shield                                                    |
| Echo Location          | ecolocalização, localização             | echolocation, echo location                                     |

---

## � **Compatibilidade**

- Compatível com mods que não alteram o sistema de reconhecimento de voz ou tradução.
- Caso utilize outros mods de tradução ou voz, pode haver conflitos. Recomenda-se testar individualmente.

---

## 💬 **Colabore!**

Sugira novos casts, variantes ou traduções:
- Abra uma [issue](https://github.com/luisgbr1el/MageArenaPortugueseMod/issues)
- Comente sobre dialetos, sinônimos ou feitiços personalizados para futuras versões.

---

## 📦 **Instalação**

1. Baixe a `.dll` da [última versão](https://github.com/luisgbr1el/MageArenaPortugueseMod/releases) do mod e coloque-a na pasta `BepInEx/plugins`.
2. Certifique-se de ter a pasta `BepInEx/plugins/LanguageModels` com o modelo [`vosk-model-small-pt-0.3`](https://alphacephei.com/vosk/models/vosk-model-small-pt-0.3.zip) dentro dela.

---

## 📚 **Documentação Técnica**

- O mod utiliza Harmony para patching e Recognissimo/Vosk para reconhecimento de voz.
- Para adicionar novos feitiços ou variantes, edite o arquivo de configuração ou contribua pelo GitHub.

---

## 🛠️ **Desenvolvimento & Contribuição**

Pull requests e sugestões são bem-vindas!
Consulte o código-fonte e documentação para detalhes sobre integração de novos comandos ou suporte a outros dialetos.
