import { InputNumberProps } from "antd";

import ClickableInputNumber from "@/components/base/ClickableInputNumber";

const clickStep = 4;

const ExerciseSizeInputNumber = (props: InputNumberProps<number>) => {
  return <ClickableInputNumber clickStep={clickStep} {...props} />;
};

export default ExerciseSizeInputNumber;
