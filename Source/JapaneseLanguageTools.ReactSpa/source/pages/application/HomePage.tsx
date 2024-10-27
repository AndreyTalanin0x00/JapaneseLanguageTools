import { Typography } from "antd";

const { Link, Paragraph, Title } = Typography;

const createExternalLink = (text: string, href: string) => {
  return (
    <Link href={href} target="_blank" rel="noopener noreferrer">
      {text}
    </Link>
  );
};

const HomePage = () => {
  return (
    <>
      <Title level={4}>{"Andrey Talanin's Japanese Language Tools"}</Title>
      <Paragraph>A set of tools for learning Japanese language: Hiragana, Katakana, Kanji hieroglyphs, numbers and words.</Paragraph>

      <Title level={5}>Project Links</Title>
      <Paragraph>{createExternalLink("GitHub-hosted project.", "https://github.com/AndreyTalanin0x00/JapaneseLanguageTools")}</Paragraph>

      <Title level={5}>Technology Stack</Title>
      <Paragraph>The project uses the following technologies and frameworks:</Paragraph>
      <Paragraph>
        <ul>
          <li>{createExternalLink("C#", "https://learn.microsoft.com/en-us/dotnet/csharp/")} is a modern general-purpose programming language for cross-platform server-side code.</li>
          <li>{createExternalLink("ASP.NET Core", "https://learn.microsoft.com/en-us/aspnet/core/")} is a framework providing a cross-platform web server with rich capabilities and extensibility options.</li>
          <li>{createExternalLink("React", "https://react.dev/")} is a client-side single page application framework.</li>
          <li>{createExternalLink("TypeScript", "https://www.typescriptlang.org/")} is a strongly-typed programming language for client-side code.</li>
          <li>{createExternalLink("Ant Design", "https://ant.design/")} is a UI component framework providing a design system.</li>
          <li>{createExternalLink("Vite", "https://vitejs.dev/")} is a client-side build tool and development server.</li>
        </ul>
      </Paragraph>
    </>
  );
};

export default HomePage;
