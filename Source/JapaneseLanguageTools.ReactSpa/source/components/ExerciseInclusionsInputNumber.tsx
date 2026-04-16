import { InputNumberProps } from "antd";

import ClickableInputNumber from "@/components/base/ClickableInputNumber";

const clickStep = 2;

const ExerciseInclusionsInputNumber = (props: InputNumberProps<number>) => {
  return <ClickableInputNumber clickStep={clickStep} {...props} />;
};

export default ExerciseInclusionsInputNumber;
