import { createBrowserRouter } from "react-router-dom";

import App from "./App";

import IntroPage from "./Pages/IntroPage";

import LoginPage from "./Pages/LoginPage";

import RegisterPage from "./Pages/RegisterPage";

import LobbyPage from "./Pages/LobbyPage";

import BarLanding from "./Pages/BarLanding";

import CoinFlip from "./Pages/games/CoinFlip";

import HigherOrLower from "./Pages/games/HigherOrLower";

import EuropeanRoulette from "./Pages/games/EuropeanRoulette";
import FarmPage from "./Pages/Biomas/FarmPage";

import ProtectedRoute from './components/Auth/ProtectedRoute';

import BlackJack from "./Pages/games/BlackJack";

import NotFound from "./Pages/error/NotFound"

import CreateOrJoinRussianRoulette from "./Pages/Components/CreateOrJoinRussianRoulette";

import LobbyRouletteRussian from "./Pages/Components/LobbyRouletteRussian";

import RussianRoulette from "./Pages/games/RussianRoulette";

import MusicRoom from "./Pages/MusicRoom";

import JoinToLobby from "./Pages/Components/JoinToLobby";
import OceanPage from "./Pages/Biomas/OceanPage";
import AnimalShop from "./Pages/AnimalShop";
import YagoMachine from "./Pages/YagoMachine";
import DesertPage from "./Pages/Biomas/DesertPage";
import JurassicPage from "./Pages/Biomas/JurassicPage";
import AnimalEditor from "./AnimalMovement/AnimalEditor";
import UrbanPage from "./Pages/Biomas/UrbanPage";
import SwampPage from "./Pages/Biomas/SwampPage";
import LegendaryPage from "./Pages/Biomas/LegendaryPage";
import SavannaPage from "./Pages/Biomas/SavannaPage";
import JunglePage from "./Pages/Biomas/JunglePage";
import RankingPage from "./Pages/RankingPage";
export const router = createBrowserRouter([

    {

        path: "/",

        element: <App />,

        children: [

            {

                path: "",

                element: <IntroPage />

            },

            {

                path: "login",

                element: <LoginPage />

            },

            {

                path: "register",

                element: <RegisterPage />

            },

            {

                path: "lobby",

                element: (
<ProtectedRoute>
<LobbyPage />
</ProtectedRoute>

                )

            },

            {

                path: "bar",

                element: (
<ProtectedRoute>
<BarLanding />
</ProtectedRoute>

                )

            }

        ]

    },

    {

        path: "/ranking",
        element: (
            <ProtectedRoute>
                <RankingPage />
            </ProtectedRoute>
        )
    },
    {
        path: "/blackjack",

        element: (
<ProtectedRoute>
<BlackJack/>
</ProtectedRoute>

        )

    },

    {

        path: "/yagomachine",

        element: (
<ProtectedRoute>
<YagoMachine />
</ProtectedRoute>

        )

    },

    {

        path: "/european-roulette",

        element: (
<ProtectedRoute>
<EuropeanRoulette />
</ProtectedRoute>
 
        )

    },

    {

        path: "/farm",

        element: (
<ProtectedRoute>
<FarmPage />
</ProtectedRoute>
 
        )

    },

    {

        path: "/ocean",

        element: (
<ProtectedRoute>
<OceanPage />
</ProtectedRoute>
 
        )

    },

    {
        path: "/desert",
        element: (
            <ProtectedRoute>
                <DesertPage />
            </ProtectedRoute>

        )
    },
    {
        path: "/jurassic",
        element: (
            <ProtectedRoute>
                <JurassicPage />
            </ProtectedRoute>
        )
    },
    {
        path: "/ocean",
        element: (
            <ProtectedRoute>
                <OceanPage />
            </ProtectedRoute>

        )
    },
    {
        path: "/shop",
        element: (
            <ProtectedRoute>
                <AnimalShop />
            </ProtectedRoute>

        )
    },
    {
        path: "/urban",
        element: (
            <ProtectedRoute>
                <UrbanPage />
            </ProtectedRoute>

        )
    },
    {
        path: "/swamp",
        element: (
            <ProtectedRoute>
                <SwampPage />
            </ProtectedRoute>

        )
    },
    {
        path: "/savanna",
        element: (
            <ProtectedRoute>
                <SavannaPage />
            </ProtectedRoute>
        )
    },
    {
        path: "/jungle",
        element: (
            <ProtectedRoute>
                <JunglePage />
            </ProtectedRoute>
        )
    },
    {
        path: "/legendary",
        element: (
            <ProtectedRoute>
                <LegendaryPage />
            </ProtectedRoute>
        )
    },
    {
    },
    {
        path: "/shop",
        element: (
            <ProtectedRoute>
                <AnimalShop />
            </ProtectedRoute>

        )
    },
    {

        path: "*",

        element: <NotFound/>

    },

    {

        path: "/coinflip",

        element: (
<ProtectedRoute>
<CoinFlip />
</ProtectedRoute>

        )

    },

    {

      path: "/higherorlower",

      element: (
        
<HigherOrLower />

      )

    },

    {

      path: "/create-or-join-russian-roulette",

      element: <CreateOrJoinRussianRoulette />

    },

    {

      path: "/lobby-russian-roulette",

      element: <LobbyRouletteRussian />

    },

    {

      path: "/join-russian-roulette",

      element: <JoinToLobby />

    },

    {

      path: "/russian-roulette/game/:lobbyId",

      element: <RussianRoulette />
    },
    {
        path: "/music-room",
        element: <MusicRoom />
    },
    {
      path: "/music-room",
      element: <MusicRoom />
    },
    {
      path: "/russian-roulette/game/:lobbyId",
      element: <RussianRoulette />
    },
    {
        path: "/higherorlower",
        element: (
            <ProtectedRoute>
                <HigherOrLower />
            </ProtectedRoute>
        )
    },
    {
        path: "/dev/animal-editor",
        element: (
            <ProtectedRoute>
                <AnimalEditor />
            </ProtectedRoute>
        )
    }
])