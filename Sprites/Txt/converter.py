import json

# Versão super simples
def converter_txt_json(arquivo_txt):
    with open(arquivo_txt, 'r') as f:
        linhas = [linha.strip() for linha in f if linha.strip()]
    
    json_data = {"sprite": linhas, "collision": "111111"}
    
    with open("sprite.json", 'w') as f:
        json.dump(json_data, f, indent=4)

# Uso
converter_txt_json("alvo.txt")