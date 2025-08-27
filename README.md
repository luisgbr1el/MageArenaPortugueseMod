
# Mage Arena Portuguese Mod

> Mod para Mage Arena que traduz todos os **textos** e permite invocar feitiços por comandos de voz em português.

Baseado no [MageArenaSpanishVoiceMod](https://github.com/S3B4S5C/MageArenaSpanishVoiceMod).

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

### Feitiços nativos do jogo:
| Feitiço (inglês)       | Cast em português                       |
|------------------------|-----------------------------------------|
| Fireball               | "fogo" ou "chamas"                      |
| Freeze                 | "congelar" ou "gelo"                    | 
| Wormhole (entrada)     | "entrada"                               |
| Wormhole (saída)       | "saída"                                 |
| Magic Missile          | "míssil mágico"                         |
| Mirror                 | "espelho, espelho meu"                  |
| Rock                   | "rocha" ou "pedra"                      |
| Wisp                   | "lento" ou "lentidão"                   |
| Dark Blast             | "explosão escura" ou "explosão"         |
| Divine Light           | "luz divina" ou "luz"                   |
| Blink                  | "clarão", "teleporte" ou "piscar"       |
| Thunderbolt            | "raio" ou "relâmpago"                   |

### Feitiços do mod/plugin [`MoreSpells`](https://thunderstore.io/c/mage-arena/p/D1GQ/MoreSpells/):
| Feitiço (inglês)       | Cast em português                       |
|------------------------|-----------------------------------------|
| Resurrection           | "ressurreição"                          |
| Magic Shield           | "escudo mágico" ou "escudo"             |
| Echo Location          | "ecolocalização" ou "localização"       |
| The Eye of Hell        | "olho do inferno" ou "olho"             |
| Hellfire               | "chama infernal"                        |

---

## � **Compatibilidade**
> **Obs: Caso sejam instalados outros mods que adicionam mais feitiços ao jogo, podem ocorrer conflitos devido o casting dos mesmos não estarem mapeados em português por esse mod.
Nesse caso, crie uma [issue](https://github.com/luisgbr1el/MageArenaPortugueseMod/issues) ou abra um Pull Request.**

- Compatível com mods que não alteram o sistema de reconhecimento de voz ou tradução.
- Caso utilize outros mods de tradução ou voz, pode haver conflitos. Recomenda-se testar individualmente.
---

## 💬 **Colabore!**

Sugira novos casts, variantes ou traduções:
- Abra uma [issue](https://github.com/luisgbr1el/MageArenaPortugueseMod/issues)
- Comente sobre dialetos, sinônimos ou feitiços personalizados para futuras versões.

---

## 📦 **Instalação**

1. Baixe o `.zip` da [última versão](https://github.com/luisgbr1el/MageArenaPortugueseMod/releases) do mod, e extraia a pasta `/MageArenaPortuguese` dentro da pasta `BepInEx/plugins`.
2. Certifique-se de ter a pasta `/LanguageModels` com o modelo [`vosk-model-small-pt-0.3`](https://alphacephei.com/vosk/models/vosk-model-small-pt-0.3.zip) dentro dela, na pasta `/MageArenaPortuguese`.

### Hierarquia dos arquivos:
```txt
  🗁 Mage Arena
  └── 🗁 BepInEx
      └── 🗁 plugins
          └── 🗁 MageArenaPortuguese
              ├── 🗁 LanguageModels
              │    └── 🗁 vosk-model-small-pt-0.3
              └── 🗐 MageArenaPortuguese.dll
```

---

## 📚 **Documentação Técnica**

- O mod utiliza Harmony para patching e Recognissimo/Vosk para reconhecimento de voz.
- Para adicionar novos feitiços ou variantes, edite o arquivo de configuração ou contribua pelo GitHub.

---

## 🛠️ **Desenvolvimento & Contribuição**

Pull requests e sugestões são bem-vindas!
Consulte o código-fonte e documentação para detalhes sobre integração de novos comandos ou suporte a outros dialetos.
