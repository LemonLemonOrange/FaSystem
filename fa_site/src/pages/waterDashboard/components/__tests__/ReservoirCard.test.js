import React from "react";
import { render, screen } from "@testing-library/react";
import ReservoirCard from "../ReservoirCard";

describe("ReservoirCard", () => {
  it("renders reservoir name, storage and percentage", () => {
    render(<ReservoirCard name="石門水庫" storage="12,691" pct={61.83} />);

    expect(screen.getByText("石門水庫")).toBeInTheDocument();
    expect(screen.getByText("蓄水量")).toBeInTheDocument();
    expect(screen.getByText("12,691")).toBeInTheDocument();
    expect(screen.getByText("萬立方公尺")).toBeInTheDocument();
    expect(screen.getByText("61.83%")).toBeInTheDocument();
  });
});