import { Button, InputNumber, InputNumberProps, Space } from "antd";
import { useCallback } from "react";

import { ArrowDownOutlined, ArrowUpOutlined } from "@ant-design/icons";

export interface ClickableInputNumberProps extends InputNumberProps<number> {
  clickStep: number;
}

const ClickableInputNumber = (props: ClickableInputNumberProps) => {
  const { value, onChange, clickStep, ...otherProps } = props;

  const modifyValue = useCallback(
    (n: number) => {
      if (!onChange || typeof value == "string") {
        return;
      }

      onChange((value ?? 0) + n);
    },
    [value, onChange]
  );

  return (
    <Space wrap direction="horizontal" align="baseline">
      <InputNumber value={value} onChange={onChange} {...otherProps} />
      <Button icon={<ArrowUpOutlined />} onClick={() => modifyValue(+clickStep)}>
        {clickStep}
      </Button>
      <Button icon={<ArrowDownOutlined />} onClick={() => modifyValue(-clickStep)}>
        {clickStep}
      </Button>
    </Space>
  );
};

export default ClickableInputNumber;
