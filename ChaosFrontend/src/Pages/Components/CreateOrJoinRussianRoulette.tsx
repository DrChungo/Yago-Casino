import { Link } from "react-router-dom";
import '../../styles/CreateOrJoinRussianRoulette.css'
import BackButton from "./BackButton";

export default function CreateOrJoinRussianRoulette() {
    return (
        <>
            <main id="menu">
                <BackButton />

                <div className="menu-title">
                    <h1>Russian Roulette</h1>
                    <div className="title-divider"></div>
                </div>

                <div className="cards-container">

                    <div className="flip-card">
                        <div className="flip-card-inner">
                            <div className="front-card">
                                <div className="card-shine"></div>
                            </div>
                            <div className="back-card">
                                <div className="corner top-left">◆</div>
                                <div className="corner bottom-right">◆</div>
                                <div className="back-glow"></div>
                                <span className="suit-icon">♠</span>
                                <Link to='/lobby-russian-roulette'>Create Game</Link>
                            </div>
                        </div>
                    </div>

                    <div className="flip-card">
                        <div className="flip-card-inner">
                            <div className="front-card">
                                <div className="card-shine"></div>
                            </div>
                            <div className="back-card">
                                <div className="corner top-left">◆</div>
                                <div className="corner bottom-right">◆</div>
                                <div className="back-glow"></div>
                                <span className="suit-icon">♦</span>
                                <Link to='/join-russian-roulette'>Join Game</Link>
                            </div>
                        </div>
                    </div>

                </div>
            </main>
        </>
    )
}