import { Typography } from "antd";
import { Link } from "react-router-dom";

const { Paragraph, Title } = Typography;

const InvalidRoutePage = () => {
  return (
    <>
      <Title level={4}>Invalid Route</Title>
      <Paragraph>There is no route corresponding to the current URL. You probably followed an invalid link, make sure you are navigating to the correct URL.</Paragraph>
      <Paragraph>If this message continues to show, contact the application developers, it may be something wrong with the application router configuration.</Paragraph>
      <Paragraph>
        You can return back to the <Link to="/home">Home page</Link> and find the contacts there.
      </Paragraph>
    </>
  );
};

export default InvalidRoutePage;
