import { useRef, useEffect, useState, useContext } from 'react'
import { useFrame, useThree } from '@react-three/fiber'
import { OrbitControls, Splat, Sky, Billboard, Text, Image } from '@react-three/drei'
import { Vector3, MOUSE } from 'three'
import { SceneContext } from '@/SceneContext.jsx'
import { UIContext } from '@/UIContext.jsx'
import { useXR } from '@react-three/xr'
import gsap from 'gsap'

export default function Scene({ targetBuildingID, setTargetBuildingID }) {
    const [splatLoaded, setSplatLoaded] = useState(false)
    const [boundingBoxes, setBoundingBoxes] = useState([])
    const [minimalBoundingBoxes, setMinimalBoundingBoxes] = useState([])
    const [error, setError] = useState(null)
    const { splatData } = useContext(SceneContext)
    const splatParentRefs = useRef([])
    const { isPresenting } = useXR()

    function generateBoundingBoxes(squareLimit = Infinity) {
        return splatParentRefs.current.map((splatParentRef, idx) => {
            if (!splatParentRef || !splatParentRef.children[0]) return null;
            
            const splat = splatParentRef.children[0]
            const splatPositions = splat.material.centerAndScaleTexture.source.data.data

            let minX = Infinity
            let maxX = -Infinity
            let minY = Infinity
            let maxY = -Infinity
            let minZ = Infinity
            let maxZ = -Infinity

            for (let i = 0; i < splatPositions.length; i += 4) {
                const x = splatPositions[i]
                const y = splatPositions[i + 1]
                const z = splatPositions[i + 2]

                if ((x > -squareLimit && x < squareLimit) && (z > -squareLimit && z < squareLimit)) {
                    if (x < minX) minX = x
                    if (x > maxX) maxX = x
                    if (y < minY) minY = y
                    if (y > maxY) maxY = y
                    if (z < minZ) minZ = z
                    if (z > maxZ) maxZ = z
                }
            }

            return { idx: idx, minX: minX, maxX: maxX, minY: minY, maxY: maxY, minZ: minZ, maxZ: maxZ }
        }).filter(box => box !== null)
    }

    useEffect(() => {
        if (splatParentRefs.current.length > 0) {
            setSplatLoaded(true)
        }
    }, [splatParentRefs.current])

    useEffect(() => {
        if (splatLoaded && splatData) {
            try {
                const generatedBoundingBoxes = generateBoundingBoxes()
                const generatedMinimalBoundingBoxes = generateBoundingBoxes(0.1)
                setBoundingBoxes(generatedBoundingBoxes)
                setMinimalBoundingBoxes(generatedMinimalBoundingBoxes)
            } catch (err) {
                console.error('Error generating bounding boxes:', err)
                setError('Failed to process 3D models')
            }
        }
    }, [splatLoaded, splatData])

    if (error) {
        return <Text position={[0, 0, 0]} color="red">{error}</Text>
    }

    if (!splatData || splatData.length === 0) {
        return <Text position={[0, 2, 0]} color="red">No splat data loaded</Text>;
    }

    return (
        <>
            <CameraRig
                splats={splatData}
                targetBuildingID={targetBuildingID}
                boundingBoxes={boundingBoxes}
            />

            <ambientLight intensity={0.5} />
            <directionalLight position={[10, 10, 5]} intensity={1} />

            <Sky sunPosition={[100, 100, 20]} />

            {/* Render only the first splat model for performance testing */}
            {splatData[0] && (
                <group ref={el => splatParentRefs.current[0] = el}>
                    <Splat
                        src={"/splats/" + splatData[0].filepath}
                        position={splatData[0].pos}
                        rotation={splatData[0].rot}
                    />
                </group>
            )}

            {/*
            {splatData.map((splat) => (
                <group key={splat.id} ref={el => splatParentRefs.current[splat.id - 1] = el}>
                    <Splat
                        src={"/splats/" + splat.filepath}
                        position={splat.pos}
                        rotation={splat.rot}
                    />
                </group>
            ))}

            {splatData.map((splat) => (
                <WaypointMarker
                    key={splat.id}
                    buildingID={splat.id}
                    targetBuildingID={targetBuildingID}
                    setTargetBuildingID={setTargetBuildingID}
                    position={splat.pos}
                    boundingBox={minimalBoundingBoxes[splat.id - 1]}
                    text={splat.name}
                />
            ))}
            */}
        </>
    )
}

function CameraRig({ splats, targetBuildingID, boundingBoxes }) {
    const { camera, gl } = useThree()
    const controls = useRef()
    const { isPresenting } = useXR()
    const movementSpeed = 0.05 // Adjust for comfort

    // Set initial camera position when entering VR
    useEffect(() => {
        if (isPresenting) {
            camera.position.set(0, 1.6, 5) // Set to typical eye height
            camera.lookAt(0, 1.6, 0)
        }
    }, [isPresenting])

    // Improved VR movement workaround using Three.js renderer's XR session
    useFrame(() => {
        if (!isPresenting) return
        const session = gl.xr && gl.xr.getSession ? gl.xr.getSession() : null
        if (!session) return
        const inputSources = Array.from(session.inputSources || [])
        const rightController = inputSources.find(src => src.handedness === 'right' && src.gamepad)
        if (rightController && rightController.gamepad && rightController.gamepad.axes.length >= 2) {
            const [x, y] = rightController.gamepad.axes
            // Deadzone
            if (Math.abs(x) > 0.1 || Math.abs(y) > 0.1) {
                // Forward/backward (y), left/right (x)
                const forward = camera.getWorldDirection(new Vector3()).setY(0).normalize()
                const right = new Vector3().crossVectors(forward, camera.up).normalize()
                camera.position.add(forward.multiplyScalar(-y * movementSpeed))
                camera.position.add(right.multiplyScalar(x * movementSpeed))
            }
        }
    })

    // Only handle OrbitControls for non-VR
    useEffect(() => {
        if (!isPresenting && boundingBoxes.length !== 0) {
            const target = splats[targetBuildingID - 1]
            const targetPosition = new Vector3(target.pos[0], target.pos[1], target.pos[2])

            // Calculate position slightly away from target (based on size of building) without derailing from dolly track
            const targetBoundingBox = boundingBoxes[targetBuildingID - 1]

            const length = Math.abs(targetBoundingBox.maxX - targetBoundingBox.minX)
            const width = Math.abs(targetBoundingBox.maxZ - targetBoundingBox.minZ)
            const height = Math.abs(targetBoundingBox.maxY - targetBoundingBox.minY)

            const scaleFactor = Math.cbrt(length * width * height) * 2

            const directionToTarget = camera.position.clone().sub(targetPosition).normalize()
            const scaledDirectionToTarget = directionToTarget.multiplyScalar(scaleFactor)
            const zoomedTargetPosition = targetPosition.clone().add(scaledDirectionToTarget)

            // Tweening camera position
            gsap.to(camera.position, {
                x: zoomedTargetPosition.x,
                y: zoomedTargetPosition.y,
                z: zoomedTargetPosition.z,
                duration: 1,
                ease: 'power3.inOut',
            })

            // Tweening camera direction
            gsap.to(controls.current.target, {
                x: targetPosition.x,
                y: targetPosition.y,
                z: targetPosition.z,
                duration: 1,
                ease: 'power3.inOut',
            })
        }
    }, [isPresenting, targetBuildingID, boundingBoxes])

    return (
        <>
            {!isPresenting && (
                <OrbitControls
                    enableDamping={true}
                    screenSpacePanning={false}
                    maxPolarAngle={Math.PI / 2.3}
                    minDistance={1}
                    maxDistance={20}
                    enablePan={true}
                    enableRotate={true}
                    enableZoom={true}
                    mouseButtons={{
                        LEFT: MOUSE.PAN,
                        MIDDLE: MOUSE.ROTATE,
                        RIGHT: MOUSE.ROTATE,
                    }}
                    rotateSpeed={1}
                    dampingFactor={1}
                    ref={controls}
                />
            )}
        </>
    )
}

function WaypointMarker({ buildingID, targetBuildingID, setTargetBuildingID, position, boundingBox, text }) {
    const [selectState, setSelectState] = useState()
    const { infoBoxVisible, setInfoBoxVisible, setShowListView } = useContext(UIContext)
    const { camera, size } = useThree()
    const billboardRef = useRef()
    const textRef = useRef()

    const desiredScreenHeight = 100

    // Ensures screen-space size of WaypointMarker is consistent 
    useFrame(() => {
        if (billboardRef.current) {
            const markerPos = billboardRef.current.position
            const cameraPos = camera.position

            // Calculate angle between camera's direction and the marker's position
            const cameraToMarker = new Vector3().subVectors(markerPos, cameraPos).normalize()
            const cameraDirection = camera.getWorldDirection(new Vector3())

            // Angle distortion correction
            const angleFactor = Math.cos(cameraToMarker.angleTo(cameraDirection))

            // Distance distortion correction
            const fovInRadians = (camera.fov * Math.PI) / 180
            const screenHeightInWorldUnits = 2 * Math.tan(fovInRadians / 2) * cameraPos.distanceTo(markerPos)

            // Adjust scale factor based on distance and angle to camera
            const scaleFactor = ((desiredScreenHeight / size.height) * screenHeightInWorldUnits) * angleFactor

            billboardRef.current.scale.set(scaleFactor, scaleFactor, scaleFactor)
        }
    })

    // Update marker position when boundingBoxes generated
    useEffect(() => {
        if (boundingBox) {
            billboardRef.current.position.y = boundingBox.maxY + position[1] + 0.1
        }
    }, [boundingBox])

    // Set selectState and color
    useEffect(() => {
        if (buildingID === targetBuildingID) {
            setSelectState("selected")
            textRef.current.color = "#E2231B"
        } else {
            setSelectState("unselected")
            textRef.current.color = "#FFFFFF"
        }
    }, [targetBuildingID])

    // Show Infobox when waypoint is clicked
    function handleClick() {
        if ((buildingID !== targetBuildingID) || !infoBoxVisible) {
            setInfoBoxVisible(true)
            setShowListView(false)
        }

        setTargetBuildingID(buildingID)
    }

    function handleHoverIn() {
        if (textRef.current) {
            textRef.current.color = "#F19199"
        }
    }

    function handleHoverOut() {
        if (textRef.current) {
            if (selectState === "selected") {
                textRef.current.color = "#E2231B"
            } else {
                textRef.current.color = "#FFFFFF"
            }
        }
    }

    return (
        <Billboard position={position} onClick={handleClick} onPointerOver={handleHoverIn} onPointerOut={handleHoverOut} ref={billboardRef}>
            <Text position={[0.2, 0, 0]} fontSize={0.17} outlineWidth={0.013} anchorX="left" ref={textRef}>
                {text}
            </Text>

            <Image
                url="images/waypoint-marker.png"
                scale={0.3}
                transparent
            />
        </Billboard>
    )
}