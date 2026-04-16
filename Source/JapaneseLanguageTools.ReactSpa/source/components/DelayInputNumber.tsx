import { InputNumberProps } from "antd";

import ClickableInputNumber from "@/components/base/ClickableInputNumber";

const clickStep = 100;

const DelayInputNumber = (props: InputNumberProps<number>) => {
  return <ClickableInputNumber clickStep={clickStep} {...props} />;
};

export default DelayInputNumber;
