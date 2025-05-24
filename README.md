# Simulado para Concursos e ENEM

![image](https://github.com/user-attachments/assets/78ac244c-392f-4309-930b-172a5fbb7c2a)


Aplicativo desktop para criação e realização de simulados com correção automática, desenvolvido em WPF (.NET 8).

## ✨ Funcionalidades

- ✅ Criação de questões com múltiplas opções
- ✅ Simulado cronometrado
- ✅ Correção automática com detalhes de desempenho
- ✅ Visualização de resultados com:
  - Tempo gasto por questão
  - Comparação entre respostas do usuário e gabarito
  - Pontuação final calculada
- ✅ Persistência de dados com SQLite

## 🚀 Como Executar

### Pré-requisitos
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQLite](https://sqlite.org/index.html) (já incluído no projeto)

### Instalação
```bash
# Clone o repositório
git clone https://github.com/seu-usuario/simulado-concursos.git

# Acesse o diretório
cd simulado-concursos

# Restaure as dependências
dotnet restore

# Execute as migrações do banco de dados
dotnet ef database update
```

### Executando o Aplicativo
```bash
dotnet run --project SimuladoConcursos
```

## 🖥️ Como Usar

1. **Adicionar Questões**
   - Acesse a tela "Adicionar Questões"
   - Preencha:
     - Enunciado da questão
     - Opções no formato:
       ```
       A Texto da opção A
       B Texto da opção B
       ```
     - Letra da resposta correta

2. **Realizar Simulado**
   - Inicie um novo simulado
   - Navegue entre as questões usando "Próxima"
   - Finalize quando chegar à última questão

3. **Ver Resultados**
   - Pontuação final
   - Tempo total gasto
   - Detalhamento por questão:
     - Sua resposta vs. Resposta correta
     - Tempo gasto por questão

## 🛠️ Tecnologias Utilizadas

- **.NET 8.0** - Framework principal
- **WPF** - Interface gráfica
- **Entity Framework Core** - ORM para acesso a dados
- **SQLite** - Banco de dados embutido
- **MVVM** - Padrão arquitetural
- **XAML** - Interface de usuário

## 🧩 Estrutura do Projeto

```
SimuladoConcursos/
├── Data/               # Configuração do banco de dados
│   └── AppDbContext.cs
├── Models/             # Entidades do sistema
│   ├── Question.cs
│   ├── Option.cs
│   └── RespostaUsuario.cs
├── ViewModels/         # Lógica de apresentação
│   └── MainViewModel.cs
├── Views/              # Telas da aplicação
│   ├── AddQuestionPage.xaml
│   ├── SimuladoPage.xaml
│   └── ResultadoPage.xaml
├── Converters/         # Conversores XAML
│   ├── BooleanToVisibilityConverter.cs
│   └── ... (outros conversores)
├── App.xaml            # Configuração global
└── MainWindow.xaml     # Janela principal
```

## 📄 Licença

Este projeto está licenciado sob a Licença MIT - veja o arquivo [LICENSE](LICENSE) para detalhes.

```text
MIT License

Copyright (c) 2024 [Seu Nome]

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

## 🤝 Como Contribuir

1. Faça um Fork do projeto
2. Crie uma Branch (`git checkout -b feature/nova-feature`)
3. Commit suas mudanças (`git commit -m 'Adiciona nova feature'`)
4. Push para a Branch (`git push origin feature/nova-feature`)
5. Abra um Pull Request

## 📌 Roadmap

- [ ] Adição de diferentes categorias de questões
- [ ] Sistema de ranking de desempenho
- [ ] Exportação/Importação de questões
- [ ] Modo de estudo por tópicos

Link do Projeto: https://github.com/dario-gms/SimuladoConcursos

Link para download do programa na versão mais recente: https://github.com/dario-gms/SimuladoConcursos/releases/tag/%23v1.0.0


